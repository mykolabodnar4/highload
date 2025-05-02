namespace FacadeService.Services;

public interface IKafkaPublisher
{
    Task Publish(FacadeApi.Grpc.Logging.Message message);
}
