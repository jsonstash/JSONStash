using JSONStash.Common.Models;
using Newtonsoft.Json.Linq;

namespace JSONStash.Common.Services.IServices
{
    public interface IJSONStashService
    {
        /// <summary>
        /// Get list of user stashes.
        /// </summary>
        /// <returns></returns>
        StashData[] GetStashes(User user);

        /// <summary>
        /// Get stash.
        /// </summary>
        /// <returns></returns>
        StashData GetStash(User user, Guid stashGuid);

        /// <summary>
        /// Create stash with record and add to collection if passed.
        /// </summary>
        /// <param name="stashName"></param>
        /// <param name="json"></param>
        /// <param name="collectionGuid"></param>
        /// <returns></returns>
        Task<StashData> CreateStash(User user, string stashName, JObject json, Guid? collectionGuid = null);

        /// <summary>
        /// Update stash data.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Task<StashData> UpdateStashData(User user, Guid stashGuid, JObject json);

        /// <summary>
        /// Update the name of the stash.
        /// </summary>
        /// <param name="stashGuid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> UpdateStashName(User user, Guid stashGuid, string name);

        /// <summary>
        /// Delete a stash.
        /// </summary>
        /// <param name="stashGuid"></param>
        /// <returns></returns>
        Task<bool> DeleteStash(User user, Guid stashGuid);
    }
}
