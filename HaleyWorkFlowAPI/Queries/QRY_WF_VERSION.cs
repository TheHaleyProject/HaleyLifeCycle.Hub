using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haley.Abstractions;
using static Haley.Abstractions.QueryFields;

namespace Haley.Models {
    public static class QRY_WF_VERSION {
        public const string INSERT = $@"INSERT INTO WF_VERSION (WORKFLOW, VERSION, DEFINITION) VALUES ({WORKFLOW}, {VERSION}, {DEFINITION}) RETURNING guid";
        public const string DELETE_BY_GUID = $@"DELETE FROM WF_VERSION WHERE GUID = {GUID}";
        public const string DELETE_BY_ID = $@"DELETE FROM WF_VERSION WHERE ID = {ID}";

        public const string NEXT_VERSION_NO = $@"SELECT COALESCE(MAX(VERSION), 0) + 1 AS NEXT_VERSION FROM WF_VERSION WHERE WORKFLOW = {WORKFLOW}";
        public const string MARK_AS_PUBLISHED = $@"UPDATE WF_VERSION SET PUBLISHED = 1 WHERE GUID = {GUID}";
       
        public const string SELECT_DEF_BY_WF_ID = $@"SELECT v.*, w.CODE, w.NAME, w.SOURCE FROM WF_VERSION v INNER JOIN WORKFLOW w ON v.WORKFLOW = w.ID WHERE v.WORKFLOW = {WORKFLOW} ORDER BY v.VERSION DESC LIMIT 1";
        public const string SELECT_DEF_BY_GUID = $@"SELECT v.*, w.CODE, w.NAME, w.SOURCE FROM WF_VERSION v INNER JOIN WORKFLOW w ON v.WORKFLOW = w.ID WHERE v.guid = {GUID}";
        public const string SELECT_DEF_BY_VER_ID = $@"SELECT v.*, w.CODE, w.NAME, w.SOURCE FROM WF_VERSION v INNER JOIN WORKFLOW w ON v.WORKFLOW = w.ID WHERE v.id = {ID}";
        public const string SELECT_DEF_BY_WF_CODE = $@"SELECT v.*, w.CODE, w.NAME, w.SOURCE FROM WF_VERSION v INNER JOIN WORKFLOW w ON v.WORKFLOW = w.ID WHERE w.SOURCE = {SOURCE} AND w.CODE = {CODE} ORDER BY v.VERSION DESC LIMIT 1";
        public const string SELECT_ID_BY_GUID = $@"SELECT id from wf_version where guid = {GUID};";
    }
}
