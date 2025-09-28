using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haley.Abstractions;
using static Haley.Abstractions.QueryFields;

namespace Haley.Models {
    public static class QRY_ENGINE {
        public const string INSERT = $@"INSERT INTO WF_ENGINE (ENVIRONMENT, GUID, STATUS) VALUES ({ENVIRONMENT}, {GUID}, {STATUS})";
        public const string SELECT_BY_GUID = $@"SELECT * FROM WF_ENGINE WHERE GUID = {GUID}";
        public const string SELECT_BY_ENVIRONMENT = $@"SELECT * FROM WF_ENGINE WHERE ENVIRONMENT = {ENVIRONMENT}";
        public const string UPDATE_BEAT = $@"UPDATE WF_ENGINE SET LAST_BEAT = CURRENT_TIMESTAMP WHERE ID = {ID}";
        public const string UPDATE_BEAT_BY_GUID = $@"UPDATE WF_ENGINE SET LAST_BEAT = CURRENT_TIMESTAMP WHERE guid = {GUID}";
        public const string MARK_DEAD = $@"UPDATE WF_ENGINE SET STATUS = 0 WHERE ID = {ID}";
        public const string MARK_DEAD_BY_GUID = $@"UPDATE WF_ENGINE SET STATUS = 0 WHERE guid = {GUID}";
        public const string SELECT_DEAD = $@"SELECT ID FROM WF_ENGINE WHERE LAST_BEAT < NOW() - INTERVAL 60 SECOND AND STATUS = 1";
    }
}
