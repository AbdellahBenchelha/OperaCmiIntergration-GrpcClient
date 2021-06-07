using CMIGrpc.Protos;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CMI_Client
{
    class  GRPC_Connection
    {
        GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:5000");
        CMIProtoService.CMIProtoServiceClient client =null;
        private string AMOUNT;

        public GRPC_Connection(string AMOUNT)
        {
            if (client == null)
            {
                this.client = new CMIProtoService.CMIProtoServiceClient(channel);
            }
            this.AMOUNT = AMOUNT;
        }

        public  async Task<string> PREAUTORISATIONAsync()
        {
          
            var reponse = await client.preautorisationAsync(
               new PaymentRequest
               {
                   TAGAMOUNT = this.AMOUNT,
                   TAGCURRENCY = "504"
               });


            return reponse.TAGCARDNUMMASK;

        }
        public async Task<string> PREAUTORISATIONConfirmationAsync()
        {

            var reponse = await client.preautorisatio_confirmationAsync(
               new PaymentRequest
               {
                   TAGAMOUNT = this.AMOUNT,
                   TAGCURRENCY = "504"
               });


            return reponse.TAGCARDNUMMASK;

        }
        public async Task<string> avoidAsync()
        {

            var reponse = await client.preautorisatio_avoidAsync(
              new PaymentRequest
              {
                  TAGAMOUNT = this.AMOUNT,
                  TAGCURRENCY = "504"
              });


            return "504";

        }
        public async Task<string> Payment()
        {

            var reponse = await client.PaymentAsync(
              new PaymentRequest
              {
                  TAGAMOUNT = this.AMOUNT,
                  TAGCURRENCY = "504"
              });


            return "504";

        }
    }
}
