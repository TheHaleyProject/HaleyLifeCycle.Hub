using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haley.Abstractions;
using static Haley.Abstractions.QueryFields;

namespace Haley.Models {
    public static class QRY_APP_SOURCE {
        public const string INSERT = $@"INSERT IGNORE INTO APP_SOURCE (CODE, NAME) VALUES ({CODE}, {NAME})";
        public const string SELECT_BY_CODE = $@"SELECT * FROM APP_SOURCE WHERE CODE = {CODE}";
        public const string EXISTS_BY_CODE = $@"SELECT 1 FROM APP_SOURCE WHERE CODE = {CODE}";
        public const string SELECT_ALL = $@"SELECT * FROM APP_SOURCE ORDER BY CODE";
        public const string UPDATE = $@"UPDATE APP_SOURCE SET NAME = {NAME} WHERE CODE = {CODE}";
        public const string DELETE = $@"DELETE FROM APP_SOURCE WHERE CODE = {CODE}";
    }

}
