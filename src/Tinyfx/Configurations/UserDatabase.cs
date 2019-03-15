namespace Tinyfx.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using Nancy;
    using Nancy.Authentication.Forms;
    using Nancy.Security;
    using Tinyfx.Cores;
    using Tinyfx.Crypto;
    using Tinyfx.Models;

    /// <summary>
    /// ��¼�û����ݶ���
    /// </summary>
    public class UserDatabase : IUserMapper
    {
        /// <summary>
        /// Ĭ���û� GUID
        /// </summary>
        private static readonly Guid _guid = Guid.Parse("C0F88B54-8780-4590-9E70-FC39D8570D4F");

        /// <summary>
        /// ��֤�û�
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Guid? ValidateUser(string username, string password)
        {

            var config = TinyfxCore.Configuration;

            if (username == config.Username && PBKDF2.Encrypt(password) == config.Password)
            {
                return _guid;
            }
            else
                return null;
        }

        /// <summary>
        /// ��ȡ�Ѿ���¼�û�
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        ClaimsPrincipal IUserMapper.GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            var config = TinyfxCore.Configuration;
            if (identifier == _guid)
            {
                return new ClaimsPrincipal(new GenericIdentity(config.Username));
            }
            else
                return null;
        }
    }
}
