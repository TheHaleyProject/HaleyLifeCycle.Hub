using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haley.Abstractions;
using static Haley.Abstractions.QueryFields;

namespace Haley.Models {
    public static class QRY_ENVIRONMENT {
        public const string INSERT = $@"INSERT IGNORE INTO ENVIRONMENT (CODE, NAME, ENGINE) VALUES ({CODE}, {NAME}, {ENGINE})";
        public const string SELECT_ALL = $@"SELECT * FROM ENVIRONMENT ORDER BY CODE";
        public const string SELECT_BY_CODE = $@"SELECT * FROM ENVIRONMENT WHERE CODE = {CODE}";
        public const string EXISTS_BY_CODE = $@"SELECT 1 FROM ENVIRONMENT WHERE CODE = {CODE}";
        public const string UPDATE_ENGINE = $@"UPDATE ENVIRONMENT SET ENGINE = {ENGINE} WHERE CODE = {CODE}";
        public const string CLEAR_ENGINE = $@"UPDATE ENVIRONMENT SET ENGINE = 0 WHERE ENGINE = {ENGINE}";
    }
}
