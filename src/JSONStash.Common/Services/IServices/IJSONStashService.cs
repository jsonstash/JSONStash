using JSONStash.Common.Models;
using Newtonsoft.Json.Linq;

namespace JSONStash.Common.Services.IServices
{
    public interface IJSONStashService
    {
        /// <summary>
        /// Get list of user stashes.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Metadata[] GetStashes(User user);

        /// <summary>
        /// Get number of versions of a stash.
        /// </summary>
        /// <param name="stashGuid"></param>
        /// <returns></returns>
        Task<Metadata> GetStashMetadata(User user, Guid stashGuid);

        /// <summary>
        /// Get stash or by version.
        /// </summary>
        /// <param name="stashGuid"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        Task<StashRecord> GetStashRecord(User user, Guid stashGuid, int? version = null);

        /// <summary>
        /// Create stash with record and add to passed collection.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="stashName"></param>
        /// <param name="json"></param>
        /// <param name="collectionGuid"></param>
        /// <returns></returns>
        Task<StashRecord> CreateStash(User user, string stashName, JObject json, Guid? collectionGuid = null);

        /// <summary>
        /// Update the name of the stash.
        /// </summary>
        /// <param name="stashGuid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> UpdateStashName(User user, Guid stashGuid, string name);

        /// <summary>
        /// Add a new record to a stash.
        /// </summary>
        /// <param name="stashGuid"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        Task<StashRecord> AddStashRecordVersion(User user, Guid stashGuid, JObject json);

        /// <summary>
        /// Delete a stash.
        /// </summary>
        /// <param name="stashGuid"></param>
        /// <returns></returns>
        Task<bool> DeleteStash(User user, Guid stashGuid);
    }
}
