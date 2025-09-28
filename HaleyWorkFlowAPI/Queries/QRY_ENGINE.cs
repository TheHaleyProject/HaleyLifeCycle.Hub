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
        public const string UPDATE_BEAT_BY_GUID = $@"UPDATE WF_ENGINE SET LAST_BEAT = CURRENT_TIMESTAMP WHERE GUID = {GUID}";
        public const string MARK_DEAD = $@"UPDATE WF_ENGINE SET STATUS = 0 WHERE ID = {ID}";
        public const string MARK_DEAD_BY_GUID = $@"UPDATE WF_ENGINE SET STATUS = 0 WHERE GUID = {GUID}";
        public const string SELECT_DEAD = $@"SELECT ID FROM WF_ENGINE WHERE LAST_BEAT < NOW() - INTERVAL 60 SECOND AND STATUS = 1";

        public const string REGISTER = $@"INSERT INTO WF_ENGINE (guid, environment, status, last_beat) VALUES ({GUID}, {ENVIRONMENT}, {STATUS}, {LAST_BEAT}) ON DUPLICATE KEY UPDATE environment = VALUES(environment), status = 1, last_beat = VALUES(last_beat)";
        public const string HEARTBEAT = $@"UPDATE WF_ENGINE SET last_beat = {LAST_BEAT}, status = 1 WHERE guid = {GUID}";
        public const string LOAD_ACTIVE = $@"SELECT id, guid, environment, last_beat, status FROM WF_ENGINE WHERE environment = {ENVIRONMENT} AND status = 1 AND last_beat >= {LAST_BEAT}";
        public const string LOAD_BY_GUID = $@"SELECT id, guid, environment, last_beat, status FROM WF_ENGINE WHERE guid = {GUID}";
        public const string RETIRE = $@"UPDATE WF_ENGINE SET status = 2 WHERE guid = {GUID}";
    }
}
