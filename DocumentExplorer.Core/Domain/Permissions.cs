using System;

namespace DocumentExplorer.Core.Domain
{
    public class Permissions
    {
        public Guid Id {get; private set;}
        public string CMR {get; private set;}
        public string FVK {get; private set;}
        public string FVP {get; private set;}
        public string NIP {get; private set;}
        public string Nota {get; private set;}
        public string PP {get; private set;}
        public string RK {get; private set;}
        public string ZK {get; private set;}
        public string ZP {get; private set;}

        public Permissions(string cmr, string fvk, string fvp, string nip, 
            string nota, string pp, string rk, string zk, string zp)
        {
            Id = Guid.NewGuid();
            SetPermissions(cmr, fvk, fvp, nip, nota, pp, rk, zk, zp);
        }

        public void SetPermissions(string cmr, string fvk, string fvp, string nip, 
            string nota, string pp, string rk, string zk, string zp)
        {
            CMR = AssignRole(cmr);
            FVK = AssignRole(fvk);
            FVP = AssignRole(fvp);
            NIP = AssignRole(nip);
            Nota = AssignRole(nota);
            PP = AssignRole(pp);
            RK = AssignRole(rk);
            ZK = AssignRole(zk);
            ZP = AssignRole(zp);
        }

        protected Permissions()
        {
            
        }
        
        private string AssignRole(string role)
        {
            if((role!=Roles.Complementer) && (role!=Roles.User))
            {
                throw new DomainException(ErrorCodes.InvalidRole);
            }
            return role;
        }

    }
}