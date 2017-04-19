using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using ExitGames.Logging;
namespace LoginServer
{
    class LoginPeer : ClientPeer
    {
        public LoginPeer(InitRequest initRequest) : base(initRequest)
        {
            LoginServer.Log("客户端上线");
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            LoginServer.Log("客户端下线");
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            LoginServer.Log("客户端发送请求");

            switch (operationRequest.OperationCode)
            {
                case (byte)OpCode.Login:
                    Login(operationRequest);
                    break;
                case (byte)OpCode.Register:
                    Register(operationRequest);
                    break;
                default:
                    break;
            }
        }

        private void Login(OperationRequest operationRequest)
        {
            string name = (string)operationRequest.Parameters[(byte)OpKey.UserName];
            string pwd = (string)operationRequest.Parameters[(byte)OpKey.Password];

            SQLiteHelper sqlite = new SQLiteHelper();

            string sql = "select * from User where user_name = '" + name.Trim() + "'";

            if (sqlite.Search(sql))
            {
                sql = "select * from User where user_name = '" + name.Trim() + "' and password = '" + pwd + "'";
                LoginServer.Log(sql);
                 
                if (sqlite.Search(sql))
                {
                    LoginServer.Log("OnOperationRequest  login  success " + name);
                    SendOperationResponse(new OperationResponse((byte)OpCode.LoginSuccess, null), new SendParameters());
                }
                else
                {
                    LoginServer.Log("OnOperationRequest  login  success with ERROR PWD");
                    SendOperationResponse(new OperationResponse((byte)OpCode.LoginFailed_PWD_ERROR, null), new SendParameters());
                }
            }
            else
            {
                LoginServer.Log("OnOperationRequest  login  failed   don't exit user with username  " + name);
                SendOperationResponse(new OperationResponse((byte)OpCode.LoginFailed_NotExitUserName, null), new SendParameters());
            }
        }

        private void Register(OperationRequest operationRequest)
        {
            string name = (string)operationRequest.Parameters[(byte)OpKey.UserName];
            string pwd = (string)operationRequest.Parameters[(byte)OpKey.Password];

            SQLiteHelper sqlite = new SQLiteHelper();

            string sql = "select * from User where user_name = '" + name.Trim() + "'";
            LoginServer.Log(sql);

            if (sqlite.Search(sql))
            {
                LoginServer.Log("OnOperationRequest  register  failed  has exit user with username " + name);
                SendOperationResponse(new OperationResponse((byte)OpCode.RegisterFailed_EXITNAME, null), new SendParameters());
            }
            else
            {
                sql = "INSERT INTO `User`(`user_name`,`password`) VALUES('" + name + "', '" + pwd + "');";
                if (sqlite.Exc(sql))
                {
                    SendOperationResponse(new OperationResponse((byte)OpCode.RegisterSuccess, null), new SendParameters());
                }
                else
                {
                    SendOperationResponse(new OperationResponse((byte)OpCode.RegisterFailed, null), new SendParameters());
                }
                LoginServer.Log(sql);
            }
        }
    }
}
