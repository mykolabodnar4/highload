package org.example;

public class HttpInfo {
    public String httpMethod;
    public String url;
    public String statusCode;

    HttpInfo(String httpMethod, String url, String statusCode) {
        this.httpMethod = httpMethod;
        this.url = url;
        this.statusCode = statusCode;
    }

    public String getFullUrl(){
        return this.httpMethod + " " + this.url;
    }

    @Override
    public String toString() {
        return httpMethod + " " + url + " " + statusCode;
    }
}
