using blogpost_website_api.DB;
using blogpost_website_api.DTO;
using blogpost_website_api.Entity;
using blogpost_website_api.Respons;
using blogpost_website_api.Security;
using MongoDB.Driver;

namespace blogpost_website_api.Service
{
    public class NotificationService
    {
        private readonly IMongoCollection<NotificationEntity> _notification;
        private readonly IMongoCollection<UserEntity> _users;
        private readonly JWT _jwt;
        public NotificationService(MongoDBContext context, JWT jwt) 
        {
            _notification = context.GetCollection<NotificationEntity>("Notification");
            _users = context.GetCollection<UserEntity>("Users");
            _jwt = jwt;
        }

        // find user's all notifications
        public async Task<respons> FindUserNotification(string token)
        {
            try
            {
                var checktoken = _jwt.ClaimDetails(token);
                if (checktoken.Success)
                {
                    var data = checktoken.Data as Dictionary<string, string>;
                    if (data == null) return new respons(false, "cant find user");
                    var UserId = data["userid"];
                    var result = await _notification.Find(u => u.UserID == UserId).SortByDescending(u => u.CreatedAt).ToListAsync();
                    return new respons(true,result);
                }
                return new respons(false,"token is invalid");
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // get all notifications 
        public async Task<respons> FindAllNotifications()
        {
            try
            {
                var allnotifications = await _notification.Find(u => true).ToListAsync();
                return new respons(true, allnotifications);
            }
            catch (Exception ex)
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // update notification states(read or not read)
        public async Task<respons> SetNotificationStates(bool isRead,string notificationID)
        {
            try
            {
                var filter = Builders<NotificationEntity>.Filter.Eq(p => p.Id, notificationID);
                var update = Builders<NotificationEntity>.Update.Set(p => p.IsRead, isRead);
                var result = await _notification.UpdateOneAsync(filter, update);
                return result.ModifiedCount > 0
                    ? new respons(true, "notification states update")
                    : new respons(false, "notification states notupdate");
                
            }
            catch (Exception ex) 
            {
                return new respons(false, "error: " + ex.Message);
            }
        }

        // Create new notification
        public async Task<respons> CreateNotification(NotificationDTO notificationDTO)
        {
            try
            {
                var notification = new NotificationEntity
                {
                    UserID = notificationDTO.UserId,
                    Title = notificationDTO.Title,
                    Message = notificationDTO.Message,
                    Link = notificationDTO.Link,
                    CreatedAt = DateTime.UtcNow
                };
                await _notification.InsertOneAsync(notification);

                var update = Builders<UserEntity>.Update.Push(u => u.NitificationID, notification.Id);
                await _users.UpdateOneAsync(u => u.Id == notificationDTO.UserId, update);

                return new respons(true,"Notification Succefully Send");
            }
            catch (Exception ex) 
            {
                return new respons(false, "error: " + ex.Message);
            } 
        }

        // send notification for all users
        public async Task<respons> SendGlobalNotification(string title, string message, string link)
        {
            try
            {
                var users = await _users.Find(_ => true).ToListAsync();
                var notifications = new List<NotificationEntity>();
                foreach (var user in users)
                {
                    var notification = new NotificationEntity
                    {
                        UserID = user.Id,
                        Title = title,
                        Message = message,
                        Link = link,
                        CreatedAt = DateTime.UtcNow
                    };

                    notifications.Add(notification);
                }
                await _notification.InsertManyAsync(notifications);
                return new respons(true, "notification send successfully");
            }
            catch (Exception ex) 
            {
                return new respons(false, "error: " + ex.Message);
            }
        }
    }
}
