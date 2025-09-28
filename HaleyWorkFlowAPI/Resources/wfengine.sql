-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               11.7.2-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             12.7.0.6850
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for smilodon
CREATE DATABASE IF NOT EXISTS `smilodon` /*!40100 DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci */;
USE `smilodon`;

-- Dumping structure for table smilodon.attempt_log
CREATE TABLE IF NOT EXISTS `attempt_log` (
  `status` int(11) NOT NULL,
  `output` longtext NOT NULL,
  `created` timestamp NOT NULL DEFAULT current_timestamp(),
  `modified` timestamp NOT NULL DEFAULT current_timestamp(),
  `engine_id` int(11) NOT NULL DEFAULT 0,
  `attempt` int(11) NOT NULL DEFAULT 1,
  `step` bigint(20) NOT NULL,
  PRIMARY KEY (`step`,`attempt`),
  KEY `fk_step_log_wf_status` (`status`),
  CONSTRAINT `fk_step_log_wf_status` FOREIGN KEY (`status`) REFERENCES `wfi_status` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_step_log_wf_step` FOREIGN KEY (`step`) REFERENCES `wfi_step` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

-- Dumping structure for table smilodon.environment
CREATE TABLE IF NOT EXISTS `environment` (
  `code` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  PRIMARY KEY (`code`),
  UNIQUE KEY `unq_environment_0` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

-- Dumping structure for table smilodon.step_belongs_to
CREATE TABLE IF NOT EXISTS `step_belongs_to` (
  `phase_id` bigint(20) NOT NULL,
  `step_id` bigint(20) NOT NULL,
  PRIMARY KEY (`step_id`,`phase_id`),
  KEY `fk_step_belongs_to_wf_group` (`phase_id`),
  CONSTRAINT `fk_step_belongs_to_wf_group` FOREIGN KEY (`phase_id`) REFERENCES `wfi_phase` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_step_belongs_to_wf_step` FOREIGN KEY (`step_id`) REFERENCES `wfi_step` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

-- Dumping structure for table smilodon.step_info
CREATE TABLE IF NOT EXISTS `step_info` (
  `message` text DEFAULT NULL,
  `output` longtext DEFAULT NULL,
  `step` bigint(20) NOT NULL,
  PRIMARY KEY (`step`),
  CONSTRAINT `fk_step_info_wfi_step` FOREIGN KEY (`step`) REFERENCES `wfi_step` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

-- Dumping structure for table smilodon.wfi_phase
CREATE TABLE IF NOT EXISTS `wfi_phase` (
  `wfi` bigint(20) NOT NULL,
  `code` int(11) NOT NULL,
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `attempt` int(11) NOT NULL DEFAULT 1,
  `status` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_wf_group_wf_instance` (`wfi`),
  KEY `fk_wf_group_wf_status` (`status`),
  CONSTRAINT `fk_wf_group_wf_instance` FOREIGN KEY (`wfi`) REFERENCES `wf_instance` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_wf_group_wf_status` FOREIGN KEY (`status`) REFERENCES `wfi_status` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=1100 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

-- Dumping structure for table smilodon.wfi_status
CREATE TABLE IF NOT EXISTS `wfi_status` (
  `code` int(11) NOT NULL,
  `name` varchar(120) NOT NULL,
  PRIMARY KEY (`code`),
  UNIQUE KEY `unq_wf_status` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

-- Dumping structure for table smilodon.wfi_step
CREATE TABLE IF NOT EXISTS `wfi_step` (
  `wfi` bigint(20) NOT NULL,
  `status` int(11) NOT NULL,
  `code` int(11) NOT NULL,
  `attempt` int(11) NOT NULL DEFAULT 1,
  `created` timestamp NOT NULL DEFAULT current_timestamp(),
  `modified` timestamp NOT NULL DEFAULT current_timestamp(),
  `timeout` timestamp NOT NULL DEFAULT current_timestamp(),
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `engine_id` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `fk_wf_step_wf_instance` (`wfi`),
  KEY `fk_wf_step_wf_status` (`status`),
  CONSTRAINT `fk_wf_step_wf_instance` FOREIGN KEY (`wfi`) REFERENCES `wf_instance` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_wf_step_wf_status` FOREIGN KEY (`status`) REFERENCES `wfi_status` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=900 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

-- Dumping structure for table smilodon.wf_engine
CREATE TABLE IF NOT EXISTS `wf_engine` (
  `environment` int(11) NOT NULL,
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `last_beat` timestamp NOT NULL DEFAULT current_timestamp() COMMENT 'heat beat every 60 seconds, to show that the engine is alive.',
  `status` int(11) NOT NULL DEFAULT 1 COMMENT '0-dead\n1-active\n2- retired',
  PRIMARY KEY (`id`),
  UNIQUE KEY `unq_environment` (`name`),
  KEY `fk_wf_engine_environment` (`environment`),
  CONSTRAINT `fk_wf_engine_environment` FOREIGN KEY (`environment`) REFERENCES `environment` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=1500 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

-- Dumping structure for table smilodon.wf_info
CREATE TABLE IF NOT EXISTS `wf_info` (
  `wfi` bigint(20) NOT NULL,
  `parameters` longtext DEFAULT NULL,
  `url_overrides` longtext DEFAULT NULL,
  `ref` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`wfi`),
  CONSTRAINT `fk_wf_info_wf_instance` FOREIGN KEY (`wfi`) REFERENCES `wf_instance` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

-- Dumping structure for table smilodon.wf_instance
CREATE TABLE IF NOT EXISTS `wf_instance` (
  `env` int(11) NOT NULL,
  `wf_version` int(11) NOT NULL,
  `guid` varchar(42) NOT NULL DEFAULT 'uuid()',
  `locked_by` int(11) NOT NULL DEFAULT 0 COMMENT 'Engine Owner',
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `created` timestamp NOT NULL DEFAULT current_timestamp(),
  `modified` timestamp NOT NULL DEFAULT current_timestamp(),
  `status` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `unq_wf_instance` (`guid`),
  KEY `fk_wf_instance_wf_version` (`wf_version`),
  KEY `fk_wf_instance_wf_status` (`status`),
  KEY `fk_wf_instance_environment` (`env`),
  CONSTRAINT `fk_wf_instance_environment` FOREIGN KEY (`env`) REFERENCES `environment` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_wf_instance_wf_status` FOREIGN KEY (`status`) REFERENCES `wfi_status` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_wf_instance_wf_version` FOREIGN KEY (`wf_version`) REFERENCES `wf_version` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=1200 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

-- Dumping structure for table smilodon.wf_version
CREATE TABLE IF NOT EXISTS `wf_version` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `version` int(11) NOT NULL,
  `definition` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL,
  `created` timestamp NOT NULL DEFAULT current_timestamp(),
  `active` bit(1) NOT NULL DEFAULT b'1',
  `published` bit(1) NOT NULL DEFAULT b'0',
  `guid` varchar(42) NOT NULL DEFAULT 'uuid()',
  `workflow` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `unq_wf_version` (`workflow`,`version`),
  UNIQUE KEY `unq_wf_version_0` (`guid`),
  KEY `fk_workflow_version_workflow` (`workflow`),
  CONSTRAINT `fk_workflow_version_workflow` FOREIGN KEY (`workflow`) REFERENCES `workflow` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `cns_wf_version_definition` CHECK (json_valid(`definition`))
) ENGINE=InnoDB AUTO_INCREMENT=1200 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

-- Dumping structure for table smilodon.workflow
CREATE TABLE IF NOT EXISTS `workflow` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `code` int(11) NOT NULL,
  `name` varchar(120) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `unq_workflow` (`name`),
  UNIQUE KEY `unq_workflow_0` (`code`)
) ENGINE=InnoDB AUTO_INCREMENT=1200 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Data exporting was unselected.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
