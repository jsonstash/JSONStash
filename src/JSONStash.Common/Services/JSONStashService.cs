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

        public Metadata[] GetStashes(User user)
        {
            if (user != null)
            {
                return user.Collections
                    .SelectMany(collection => collection.Stashes
                    .Select(stash => new Metadata(stash)))
                    .OrderByDescending(metadata => metadata.Created)
                    .ToArray();
            }

            return null;
        }

        public async Task<StashRecord> AddStashRecordVersion(User user, Guid stashGuid, JObject json)
        {
            Stash stash = await _context.Stashes.FirstOrDefaultAsync(stash => stash.StashGuid.Equals(stashGuid) && stash.Collection.User.UserGuid.Equals(user.UserGuid));

            if (stash != null)
            {
                Record record = new();

                Metadata metadata = await GetStashMetadata(user, stashGuid);

                record.StashId = stash.StashId;
                record.Version = metadata.Versions + 1;
                record.Value = json.ToString(Formatting.None);
                record.Created = DateTimeOffset.UtcNow;

                await _context.Records.AddAsync(record);
                await _context.SaveChangesAsync();

                return new(stash, record.Value);
            }

            return null;
        }

        public async Task<StashRecord> CreateStash(User user, string stashName, JObject json, Guid? collectionGuid = null)
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
                    Created = DateTimeOffset.UtcNow,
                    StashGuid = Guid.NewGuid(),
                };

                await _context.Stashes.AddAsync(stash);
                await _context.SaveChangesAsync();

                Record record = new()
                {
                    StashId = stash.StashId,
                    Version = 1,
                    Value = json.ToString(Formatting.None),
                    Created = DateTimeOffset.UtcNow
                };

                await _context.Records.AddAsync(record);
                await _context.SaveChangesAsync();

                return new(stash, record.Value);
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

        public async Task<StashRecord> GetStashRecord(User user, Guid stashGuid, int? version = null)
        {
            Stash stash = await _context.Stashes.FirstOrDefaultAsync(stash => stash.StashGuid.Equals(stashGuid) && stash.Collection.User.UserGuid.Equals(user.UserGuid));

            if (stash != null)
            {
                Metadata metadata = await GetStashMetadata(user, stashGuid);

                if (version != null)
                {
                    if (version <= metadata.Versions)
                    {
                        Record record = stash.Records.FirstOrDefault(record => record.Version == version);
                        return new(stash, record.Value);
                    }
                }
                else
                {
                    Record record = stash.Records.OrderByDescending(record => record.Created).FirstOrDefault();
                    return new(stash, record.Value);
                }
                
            }

            return null;
        }

        public async Task<Metadata> GetStashMetadata(User user, Guid stashGuid)
        {
            Stash stash = await _context.Stashes.FirstOrDefaultAsync(stash => stash.StashGuid.Equals(stashGuid) && stash.Collection.User.UserGuid.Equals(user.UserGuid));

            if (stash != null)
                return new(stash);

            return null;
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
    }
}
