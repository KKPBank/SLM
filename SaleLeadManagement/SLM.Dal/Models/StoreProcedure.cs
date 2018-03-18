using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Dal.Models
{
    public class StoreProcedure
    {
        public string GenerateTicketId()
        {
            try
            {
                using (SLM_DBEntities slmdb = DBUtil.GetSlmDbEntities())
                {
                    var ticketId = slmdb.kkslm_sp_generate_ticket_id().Select(p => p.Trim()).FirstOrDefault();
                    if (ticketId != null)
                        return ticketId;
                    else
                        throw new Exception("Ticket ID cannot be generated.");
                }
            }
            catch
            {
                throw;
            }
        }

        public string GenerateTicketId(SLM_DBEntities slmdb)
        {
            try
            {
                var ticketId = slmdb.kkslm_sp_generate_ticket_id().Select(p => p.Trim()).FirstOrDefault();
                if (ticketId != null)
                    return ticketId;
                else
                    throw new Exception("Ticket ID cannot be generated.");
            }
            catch
            {
                throw;
            }
        }
    }
}
