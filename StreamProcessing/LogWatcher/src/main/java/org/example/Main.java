package org.example;

import java.util.Map;
import java.util.concurrent.TimeUnit;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.hazelcast.core.Hazelcast;
import com.hazelcast.jet.aggregate.AggregateOperations;
import com.hazelcast.jet.pipeline.Pipeline;
import com.hazelcast.jet.pipeline.Sinks;
import com.hazelcast.jet.pipeline.Sources;
import com.hazelcast.jet.pipeline.StreamStage;
import com.hazelcast.jet.pipeline.WindowDefinition;
import com.hazelcast.map.EntryProcessor;

public class Main {
    public static void main(String[] args) {
        var logFile = "E:\git\highload\StreamProcessing\simple-api";

        var pipeline = Pipeline.create();
        StreamStage<HttpInfo> filteredSource = pipeline
            .readFrom(Sources.fileWatcher(logFile))
            .withIngestionTimestamps()
            .map(s -> getMethodInfo(s))
            .filter(x -> x.url != null && x.httpMethod != null && x.statusCode != null)
            .filter(x -> x.statusCode.equals("200"));
        
        filteredSource
            .map(info -> Map.entry(info.getFullUrl(), info.statusCode))
            .setName("log file watcher")
            .writeTo(Sinks.mapWithEntryProcessor(
                "successfulApiCalls",
                entry -> entry.getKey(),
                entry -> new IncrementEntryProcessor())
            );

        filteredSource
            .groupingKey(info -> info.getFullUrl())
            .window(WindowDefinition.tumbling(TimeUnit.SECONDS.toMillis(30)))
            .aggregate(AggregateOperations.counting())
            .writeTo(Sinks.logger());

        var hzInstance = Hazelcast.bootstrappedInstance();

        hzInstance.getJet().newJob(pipeline);
    }

    public static HttpInfo getMethodInfo(String logString) {
        var httpMethodPattern = Pattern.compile("(GET|POST|PUT|DELETE|PATCH)");
        var urlPattern = Pattern.compile("(http://[^\\s]+)");
        var statusCodePattern = Pattern.compile("\\s(\\d{3})\\s");

        Matcher httpMethodMatcher = httpMethodPattern.matcher(logString);
        Matcher urlMatcher = urlPattern.matcher(logString);
        Matcher statusCodeMatcher = statusCodePattern.matcher(logString);

        String httpMethod = httpMethodMatcher.find() ? httpMethodMatcher.group(1) : null;
        String url = urlMatcher.find() ? urlMatcher.group(1) : null;
        String statusCode = statusCodeMatcher.find() ? statusCodeMatcher.group(1) : null;
        
        System.out.println(new HttpInfo(httpMethod, url, statusCode));
        return new HttpInfo(httpMethod, url, statusCode);
    }

    static class IncrementEntryProcessor implements EntryProcessor<String, Integer, Integer> {
        @Override
        public Integer process(Map.Entry<String, Integer> entry) {
            var value = entry.getValue() == null ? 0 : entry.getValue();
            return entry.setValue(value + 1);
        }
    }
}