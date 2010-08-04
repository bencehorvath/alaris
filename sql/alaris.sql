/*
MySQL Data Transfer

Source Server         : Local Development
Source Server Version : 50141
Source Host           : localhost:3306
Source Database       : alaris

Target Server Type    : MYSQL
Target Server Version : 50141
File Encoding         : 65001

Date: 2010-08-04 22:02:51
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for `messages`
-- ----------------------------
DROP TABLE IF EXISTS `messages`;
CREATE TABLE `messages` (
  `id` int(30) unsigned NOT NULL AUTO_INCREMENT,
  `msg` longtext COLLATE latin2_hungarian_ci NOT NULL COMMENT 'Message to send',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=3 DEFAULT CHARSET=latin2 COLLATE=latin2_hungarian_ci;

-- ----------------------------
-- Records of messages
-- ----------------------------
INSERT INTO messages VALUES ('1', 'MySQL adatbázis teszt.');
INSERT INTO messages VALUES ('2', 'Hello, I\'m Alaris!');
