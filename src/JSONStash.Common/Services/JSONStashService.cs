using JSONStash.Common.Context;
using JSONStash.Common.Models;
using JSONStash.Common.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace JSONStash.Common.Services
{
    public class JSONStashService : IJSONStashService
    {
        private readonly JSONStashContext _context;

        public JSONStashService(JSONStashContext context)
        {
            _context = context;
        }

        public StashDetail[] GetStashes(User user)
        {
            if (user != null)
            {
                return user.Collections
                    .SelectMany(collection => collection.Stashes
                    .Select(stash => new StashDetail(stash)))
                    .OrderByDescending(stashData => stashData.Created)
                    .ToArray();
            }

            return null;
        }

        public async Task<StashDetail> CreateStash(User user, string stashName, JObject json, Guid? collectionGuid = null)
        {
            if (user != null)
            {
                Collection collection = user.Collections.FirstOrDefault(collection => collection.CollectionGuid.Equals(collectionGuid));

                if (collection == null)
                {
                    collection = new()
                    {
                        UserId = user.UserId,
                        Created = DateTimeOffset.UtcNow,
                        CollectionGuid = Guid.NewGuid()
                    };

                    await _context.Collections.AddAsync(collection);
                    await _context.SaveChangesAsync();
                }

                Stash stash = new()
                {
                    Name = stashName,
                    CollectionId = collection.CollectionId,
                    Data = json.ToString(Formatting.None),
                    Key = Guid.NewGuid(),
                    Created = DateTimeOffset.UtcNow,
                    StashGuid = Guid.NewGuid(),
                };

                await _context.Stashes.AddAsync(stash);
                await _context.SaveChangesAsync();

                return new(stash);
            }

            return null;
        }

        public async Task<bool> DeleteStash(User user, Guid stashGuid)
        {
            Stash stash = await _context.Stashes.FirstOrDefaultAsync(stash => stash.StashGuid.Equals(stashGuid) && stash.Collection.User.UserGuid.Equals(user.UserGuid));

            if (stash != null)
            {
                bool isLastStash = stash.Collection.Stashes.Count == 1;

                if (isLastStash)
                {
                    _context.Collections.Remove(stash.Collection);
                    _context.Stashes.Remove(stash);
                }
                else
                {
                    _context.Stashes.Remove(stash);
                }
                
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateStashName(User user, Guid stashGuid, string name)
        {
            Stash stash = await _context.Stashes.FirstOrDefaultAsync(stash => stash.StashGuid.Equals(stashGuid) && stash.Collection.User.UserGuid.Equals(user.UserGuid));

            if (stash != null)
            {
                stash.Name = name;
                stash.Modified = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<StashData> UpdateStashData(Guid stashGuid, JObject json)
        {
            Stash stash = await _context.Stashes.FirstOrDefaultAsync(stash => stash.StashGuid.Equals(stashGuid));

            if (stash != null)
            {
                stash.Data = json.ToString(Formatting.None);
                stash.Modified = DateTimeOffset.UtcNow;

                await _context.SaveChangesAsync();

                return new(stash);
            }

            return null;
        }

        public async Task<StashData> GetStash(Guid stashGuid)
        {
            Stash stash = await _context.Stashes.FirstOrDefaultAsync(stash => stash.StashGuid.Equals(stashGuid));

            if (stash != null)
                return new(stash);

            return null;
        }
    }
}
