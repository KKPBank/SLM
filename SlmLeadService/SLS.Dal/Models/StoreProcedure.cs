using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLS.Resource;

namespace SLS.Dal.Models
{
    public class StoreProcedure
    {
        private SLM_DBEntities slmdb = null;

        public StoreProcedure(SLM_DBEntities db)
        {
            slmdb = db;
        }

        public string GenerateTicketId()
        {
            try
            {
                var ticketId = slmdb.kkslm_sp_generate_ticket_id().Select(p => p.Trim()).FirstOrDefault();
                if (ticketId != null)
                {
                    return ticketId;
                } 
                else
                {
                    throw new ServiceException(ApplicationResource.INS_ERROR_FROM_DB_CODE, ApplicationResource.INS_ERROR_FROM_DB_DESC, "Ticket ID cannot be generated.", null);
                } 
            }
            catch (Exception ex)
            {
                throw new ServiceException(ApplicationResource.INS_ERROR_FROM_DB_CODE, ApplicationResource.INS_ERROR_FROM_DB_DESC, ex.Message, ex.InnerException);
            }
        }
    }
}
