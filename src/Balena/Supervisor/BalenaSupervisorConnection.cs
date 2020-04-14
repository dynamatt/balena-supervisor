namespace Balena.Supervisor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Models;

    public class BalenaSupervisorConnection
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Create a connection to the Balena supervisor using the parameters found in the
        /// environment variables.
        /// </summary>
        public BalenaSupervisorConnection()
            : this(DefaultSupervisorUrl, BalenaSupervisorApiKey)
        {

        }

        public BalenaSupervisorConnection(string address, string apiKey)
        {
            _client = CreateClient(address, apiKey);
        }

        public static BalenaSupervisorConnection CreateFromApplicationContainer()
        {
            return CreateFromApplicationContainer(DefaultSupervisorUrl, BalenaSupervisorApiKey);
        }

        public static BalenaSupervisorConnection CreateFromApplicationContainer(string address, string apiKey)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">The address of the Balena Cloud or Open Balena endpoint.</param>
        /// <param name="apiToken">Balena Cloud API Token</param>
        /// <param name="uuid">A uuid or deviceId, to query a specific device, or an applicationId to query all devices in an application.</param>
        /// <returns></returns>
        public static BalenaSupervisorConnection CreateFromCloudApi(string address, string apiToken, string uuid)
        {
            throw new NotImplementedException();
        }

        public static string DefaultSupervisorUrl
        {
            get
            {
                return Environment.GetEnvironmentVariable("BALENA_SUPERVISOR_ADDRESS");
            }
        }

        public static string BalenaSupervisorApiKey
        {
            get
            {
                return Environment.GetEnvironmentVariable("BALENA_SUPERVISOR_API_KEY");
            }
        }

        private static HttpClient CreateClient(string url, string apiToken)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            
            // base address must end in /, see https://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
            if (url.Last() != '/')
            {
                url = url + '/';
            }
            client.BaseAddress = new Uri(url);
            return client;
        }

        /// <summary>
        /// Pings the supervisor and throws an exception if an "OK" message is not received.
        /// </summary>
        public async Task Ping()
        {
            await _client.ExecuteAsync("ping");
        }

        /// <summary>
        /// Starts a blink pattern on a LED for 15 seconds, if your device has one. 
        /// It implements the "identify device" feature from the dashboard.
        /// Throws an exception if an "OK" message is not received.
        /// </summary>
        public async Task Blink()
        {
            await _client.ExecuteAsync("v1/blink");
        }

        /// <summary>
        //// Triggers an update check on the supervisor. Optionally, forces an update when updates are locked.
        /// </summary>
        public async Task Update(bool force=false)
        {
            if (force)
            {
                throw new NotImplementedException("Forced update has not been implemented.");
            }
            await _client.ExecuteAsync("v1/update");
        }

        /// <summary>
        /// Reboots the device. This will first try to stop applications, and fail if there is an update lock.
        /// An optional "force" parameter in the body overrides the lock when true (and the lock can also be overridden from the dashboard).
        /// </summary>
        public async Task Reboot(bool force=false)
        {
            if (force)
            {
                throw new NotImplementedException("Forced reboot has not been implemented.");
            }
            await _client.ExecuteAsync("v1/reboot");
        }

        /// <summary>
        /// Dangerous. Shuts down the device. This will first try to stop applications, and fail if there is an update lock. 
        /// An optional "force" parameter in the body overrides the lock when true (and the lock can also be overridden from the dashboard).        /// </summary>
        /// </summary>
        public async Task Shutdown(bool force=false)
        {
            if (force)
            {
                throw new NotImplementedException("Forced shutdown has not been implemented.");
            }
            await _client.ExecuteAsync("v1/shutdown");
        }

        /// <summary>
        /// Restarts a user application container.
        /// </summary>
        public async Task Restart(int appid)
        {
            throw new NotImplementedException("Restart has not been implemented.");
        }

        /// <summary>
        /// Returns the current device state, as reported to the balenaCloud API and with some extra fields added to allow control over pending/locked updates.
        /// </summary>
        public async Task<DeviceInfo> Device()
        {
            return await _client.ExecuteAsync<DeviceInfo>("v1/device");
        }

        /// <summary>
        /// Returns the application running on the device.
        /// </summary>
        public async Task<ApplicationInfo> Application(int appId)
        {
            return await _client.ExecuteAsync<ApplicationInfo>($"v1/apps/{appId}");
        }

        /// <summary>
        /// Used internally to check whether the supervisor is running correctly, according to some heuristics that help determine whether the internal components, application updates and reporting to the balenaCloud API are functioning.
        /// </summary>
        public async Task Healthy()
        {
            await _client.ExecuteAsync("v1/healthy");
        }
    }

    public class ApplicationInfo
    {
        /// <summary>
        /// The id of the app as per the balenaCloud API.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Application commit that is running.
        /// </summary>
        public string Commit { get; set; }

        /// <summary>
        /// The docker image of the current application build.
        /// </summary>
        public string ImageId { get; set; }

        /// <summary>
        /// ID of the docker container of the running app.
        /// </summary>
        public string ContainerId { get; set; }

        /// <summary>
        /// A key-value store of the app's environment variables.
        /// </summary>
        public string Env { get; set; }
    }

    public class DeviceInfo
    {
        /// <summary>
        /// Port on which the supervisor is listening.
        /// </summary>
        public int ApiPort { get; set; }

        /// <summary>
        /// Hash of the current commit of the application that is running.
        /// </summary>
        public string Commit { get; set; }

        /// <summary>
        /// Space-separated list of IP addresses of the device.
        /// </summary>
        public string IPAddress { get; set;}

        /// <summary>
        /// Status of the device regarding the app, as a string, i.e. "Stopping", "Starting", "Downloading", "Installing", "Idle".
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Amount of the application image that has been downloaded, expressed as a percentage. If the update has already been downloaded, this will be null.
        /// </summary>
        public double DownloadProgress { get; set; } 

        /// <summary>
        /// Version of the host OS running on the device.
        /// </summary>
        public string OSVersion { get; set; }

        /// <summary>
        /// Version of the supervisor running on the device.
        /// </summary>
        public string SupervisorVersion { get; set; } 

        /// <summary>
        /// A boolean that will be true if the supervisor has detected there is a pending update.
        /// </summary>
        public bool UpdatePending { get; set; } 

        /// <summary>
        /// Boolean that will be true if a pending update has already been downloaded.
        /// </summary>
        public bool UpdateDownloaded { get; set; }

        /// <summary>
        /// Boolean that will be true if the supervisor has tried to apply a pending update but failed (i.e. if the app was locked, there was a network failure or anything else went wrong).
        /// </summary>
        public bool UpdateFailed {get; set; }
    }
}