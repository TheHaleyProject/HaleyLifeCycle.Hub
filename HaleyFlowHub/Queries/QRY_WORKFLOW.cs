using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haley.Abstractions;
using static Haley.Abstractions.QueryFields;

namespace Haley.Models {
    public static class QRY_WORKFLOW {
        public const string INSERT = $@"INSERT IGNORE INTO WORKFLOW (SOURCE, CODE, NAME) VALUES ({SOURCE}, {CODE}, {NAME})";
        public const string SELECT_BY_ID = $@"SELECT * FROM WORKFLOW WHERE ID = {ID}";
        public const string EXISTS_BY_ID = $@"SELECT 1 FROM WORKFLOW WHERE ID = {ID}";
        public const string EXISTS_BY_CODE = $@"SELECT 1 FROM WORKFLOW WHERE SOURCE = {SOURCE} AND CODE = {CODE}";
        public const string SELECT_BY_CODE = $@"SELECT * FROM WORKFLOW WHERE SOURCE = {SOURCE} AND CODE = {CODE}";
        public const string GET_ID_BY_CODE = $@"SELECT ID FROM WORKFLOW WHERE SOURCE = {SOURCE} AND CODE = {CODE}";
        public const string SELECT_ALL = $@"SELECT * FROM WORKFLOW ORDER BY SOURCE, CODE";
        public const string SELECT_BY_SOURCE = $@"SELECT * FROM WORKFLOW WHERE SOURCE = {SOURCE} OR SOURCE = 0 ORDER BY CODE";
        public const string UPDATE = $@"UPDATE IGNORE WORKFLOW SET NAME = {NAME} WHERE SOURCE = {SOURCE} AND CODE = {CODE}";
        public const string DELETE = $@"DELETE FROM WORKFLOW WHERE SOURCE = {SOURCE} AND CODE = {CODE}";
        public const string GET_GUID_BY_CODE = $@"SELECT v.guid FROM WF_VERSION v INNER JOIN WORKFLOW w ON v.WORKFLOW = w.ID WHERE w.SOURCE = {SOURCE} AND w.CODE = {CODE} ORDER BY v.VERSION DESC LIMIT 1";
    }
}
