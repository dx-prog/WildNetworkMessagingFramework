namespace WNMF.Common.Definition {
    public interface ISimpleNetworkMessageReceiver : INServiceProvider {
        /// <summary>
        /// Gets the name of the this agent
        /// </summary>
        string AgentName { get; }

        /// <summary>
        /// Moves from the endpoint to local cache, and returns a stream that can be used to access
        /// data from cache
        /// the data
        /// </summary>
        /// <param name="result"></param>
        bool Stage(TryOperationResponse<INetworkMessageStream> result);
    }
}