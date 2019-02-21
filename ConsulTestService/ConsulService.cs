using System;
using System.Net;
using Consul;
using Serilog;

namespace ConsulTestService
{
    internal sealed class ConsulService
    {
        
        private ILogger Logger => Log.ForContext<ConsulService>();

        public async void RegisterDiscoveryService()
        {
            try
            {
                using (var consul = new ConsulClient())
                {
                    var name = ServiceConfigs.ServiceName;
                    var port = ServiceConfigs.ServicePort;
                    var id = $"{name}:{port}";

                    var tags = new[] { "debug" };
                    var deregisterCriticalServiceAfter = TimeSpan.FromSeconds(5);

                    var result = await consul.Agent.ServiceRegister(
                        new AgentServiceRegistration
                        {
                            ID = id,
                            Name = name,
                            Port = port,
                            Address = "127.0.0.1",
                            Tags = tags,
                            Check = new AgentServiceCheck
                            {
                                DeregisterCriticalServiceAfter = deregisterCriticalServiceAfter,
                                Interval = TimeSpan.FromSeconds(5),
                                HTTP = $"http://localhost:{port}/ping"
                            },
                        });

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        Logger.Information("Сервис {service} зарегистрирован в Consul.", name);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "ошибка при регистрации в Consul");
            }
        }

        public async void DeregisterDiscoveryService()
        {
            
            try
            {
                using (var consul = new ConsulClient())
                {
                    var name = ServiceConfigs.ServiceName;
                    var port = ServiceConfigs.ServicePort;
                    var id = $"{name}:{port}";
                    var result = await consul.Agent.ServiceDeregister(id);
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        Logger.Information("Сервис {service} дерегистрирован из Consul.", id);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при попытке дерегистировать сервис");
            }
        }
    }
}
