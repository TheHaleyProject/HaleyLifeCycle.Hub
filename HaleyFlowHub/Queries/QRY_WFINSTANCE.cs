using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haley.Abstractions;
using static Haley.Abstractions.QueryFields;

namespace Haley.Models {
    public static class QRY_WFINSTANCE {
        // 🔹 WF_INSTANCE
        public const string INSERT_INSTANCE = $@"INSERT INTO WF_INSTANCE (GUID, WF_VERSION, ENV, LOCKED_BY, STATUS) 
                                             VALUES ({GUID}, {WF_VERSION}, {ENVIRONMENT}, {LOCKED_BY}, {STATUS}) RETURNING ID";

        public const string SELECT_BY_GUID = $@"SELECT * FROM WF_INSTANCE WHERE GUID = {GUID}";
        public const string SELECT_BY_ID = $@"SELECT * FROM WF_INSTANCE WHERE ID = {ID}";
        public const string SELECT_ACTIVE_BY_ENGINE = $@"SELECT * FROM WF_INSTANCE WHERE LOCKED_BY = {LOCKED_BY} AND ENV = {ENVIRONMENT}";
        public const string RELEASE_BY_ENGINE = $@"UPDATE WF_INSTANCE SET LOCKED_BY = 0 WHERE LOCKED_BY = {LOCKED_BY}";

        public const string CLAIM_ORPHANED = $@"UPDATE WF_INSTANCE 
                                            SET LOCKED_BY = {LOCKED_BY}, MODIFIED = CURRENT_TIMESTAMP 
                                            WHERE ENV = {ENVIRONMENT} 
                                              AND (LOCKED_BY = 0 OR LOCKED_BY IN (
                                                  SELECT ID FROM WF_ENGINE 
                                                  WHERE LAST_BEAT < NOW() - INTERVAL 60 SECOND AND STATUS = 1
                                              )) 
                                              AND STATUS IN ({STATUS_LIST}) 
                                            LIMIT {LIMIT}";

        public const string UPDATE_STATUS = $@"UPDATE WF_INSTANCE SET STATUS = {STATUS}, MODIFIED = CURRENT_TIMESTAMP WHERE ID = {ID}";
        public const string UPDATE_LOCK = $@"UPDATE WF_INSTANCE SET LOCKED_BY = {LOCKED_BY}, MODIFIED = CURRENT_TIMESTAMP WHERE ID = {ID}";

        // 🔹 WFI_BELONGS_TO
        public const string INSERT_BELONGS_TO = $@"INSERT INTO WFI_BELONGS_TO (WFI_ID, SOURCE, OWNER, REF) 
                                               VALUES ({WFI_ID}, {SOURCE}, {OWNER}, {REF})";

        public const string SELECT_BELONGS_TO = $@"SELECT * FROM WFI_BELONGS_TO WHERE WFI_ID = {WFI_ID}";
        public const string DELETE_BELONGS_TO = $@"DELETE FROM WFI_BELONGS_TO WHERE WFI_ID = {WFI_ID}";

        // 🔹 WF_INFO
        public const string INSERT_INFO = $@"INSERT INTO WF_INFO (WFI, PARAMETERS, URL_OVERRIDES) 
                                         VALUES ({WFI}, {PARAMETERS}, {URL_OVERRIDES})";

        public const string SELECT_INFO = $@"SELECT * FROM WF_INFO WHERE WFI = {WFI}";
        public const string UPDATE_INFO = $@"UPDATE WF_INFO SET PARAMETERS = {PARAMETERS}, URL_OVERRIDES = {URL_OVERRIDES} WHERE WFI = {WFI}";
    }

}
