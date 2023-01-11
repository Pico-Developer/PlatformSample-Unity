using Pico.Platform.Models;
using UnityEngine;

namespace Pico.Platform.Samples
{
    public class GetAllFriends
    {
        public delegate void HandleGetAllUsers(UserList userList);

        public delegate void HandleError(Error error);

        public static void Run(HandleGetAllUsers handleGetAllUsers, HandleError handleError)
        {
            UserService.GetFriends().OnComplete(m =>
            {
                if (m.IsError)
                {
                    handleError(m.Error);
                    return;
                }

                submit(m.Data, handleGetAllUsers, handleError);
            });
        }

        private static void submit(UserList userList, HandleGetAllUsers handleGetAllUsers, HandleError handleError)
        {
            if (userList.HasNextPage)
            {
                Debug.Log("has next page");
                UserService.GetNextUserListPage(userList).OnComplete(m =>
                {
                    if (m.IsError)
                    {
                        handleError(m.Error);
                        return;
                    }

                    m.Data.InsertRange(0, userList);
                    submit(m.Data, handleGetAllUsers, handleError);
                });
            }
            else
            {
                handleGetAllUsers(userList);
            }
        }
    }
}