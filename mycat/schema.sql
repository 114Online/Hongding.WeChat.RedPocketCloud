/*
Navicat MySQL Data Transfer

Source Server         : localhost_3306
Source Server Version : 50713
Source Host           : localhost:3306
Source Database       : wechat

Target Server Type    : MYSQL
Target Server Version : 50713
File Encoding         : 65001

Date: 2016-08-11 22:30:29
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for activities
-- ----------------------------
DROP TABLE IF EXISTS `Activities`;
CREATE TABLE `Activities` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Attend` bigint(20) NOT NULL,
  `Begin` datetime NOT NULL,
  `BriberiesCount` bigint(20) NOT NULL,
  `End` datetime DEFAULT NULL,
  `IsBegin` bit(1) NOT NULL,
  `Limit` int(11) NOT NULL,
  `MerchantId` bigint(20) NOT NULL,
  `Price` bigint(20) NOT NULL,
  `Ratio` double NOT NULL,
  `Rules` json DEFAULT NULL,
  `TemplateId` bigint(20) NOT NULL,
  `Title` longtext,
  PRIMARY KEY (`Id`),
  KEY `IX_Activities_Attend` (`Attend`),
  KEY `IX_Activities_Begin` (`Begin`),
  KEY `IX_Activities_End` (`End`),
  KEY `IX_Activities_IsBegin` (`IsBegin`),
  KEY `IX_Activities_MerchantId` (`MerchantId`),
  KEY `IX_Activities_Price` (`Price`)
);

-- ----------------------------
-- Table structure for aspnetroleclaims
-- ----------------------------
DROP TABLE IF EXISTS `AspNetRoleClaims`;
CREATE TABLE `AspNetRoleClaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  `RoleId` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for aspnetroles
-- ----------------------------
DROP TABLE IF EXISTS `AspNetRoles`;
CREATE TABLE `AspNetRoles` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `ConcurrencyStamp` longtext,
  `Name` varchar(256) DEFAULT NULL,
  `NormalizedName` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `RoleNameIndex` (`NormalizedName`)
);

-- ----------------------------
-- Table structure for aspnetuserclaims
-- ----------------------------
DROP TABLE IF EXISTS `AspNetUserClaims`;
CREATE TABLE `AspNetUserClaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  `UserId` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for aspnetuserlogins
-- ----------------------------
DROP TABLE IF EXISTS `AspNetUserLogins`;
CREATE TABLE `AspNetUserLogins` (
  `LoginProvider` varchar(255) NOT NULL,
  `ProviderKey` varchar(255) NOT NULL,
  `ProviderDisplayName` longtext,
  `UserId` bigint(20) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for aspnetuserroles
-- ----------------------------
DROP TABLE IF EXISTS `AspNetUserRoles`;
CREATE TABLE `AspNetUserRoles` (
  `UserId` bigint(20) NOT NULL,
  `RoleId` bigint(20) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  KEY `IX_AspNetUserRoles_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
);

-- ----------------------------
-- Table structure for aspnetusers
-- ----------------------------
DROP TABLE IF EXISTS `AspNetUsers`;
CREATE TABLE `AspNetUsers` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `AccessFailedCount` int(11) NOT NULL,
  `Balance` double NOT NULL,
  `ConcurrencyStamp` longtext,
  `Email` varchar(256) DEFAULT NULL,
  `EmailConfirmed` bit(1) NOT NULL,
  `Limit` int(11) NOT NULL,
  `LockoutEnabled` bit(1) NOT NULL,
  `LockoutEnd` varchar(255) DEFAULT NULL,
  `Merchant` longtext,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `NormalizedUserName` varchar(256) DEFAULT NULL,
  `PasswordHash` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` bit(1) NOT NULL,
  `SecurityStamp` longtext,
  `TwoFactorEnabled` bit(1) NOT NULL,
  `UserName` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
);

-- ----------------------------
-- Table structure for aspnetusertokens
-- ----------------------------
DROP TABLE IF EXISTS `AspNetUserTokens`;
CREATE TABLE `AspNetUserTokens` (
  `UserId` bigint(20) NOT NULL,
  `LoginProvider` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Value` longtext,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`)
);

-- ----------------------------
-- Table structure for blobs
-- ----------------------------
DROP TABLE IF EXISTS `Blobs`;
CREATE TABLE `Blobs` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Bytes` longblob,
  `ContentLength` bigint(20) NOT NULL,
  `ContentType` varchar(128) DEFAULT NULL,
  `FileName` varchar(128) DEFAULT NULL,
  `Time` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Blobs_FileName` (`FileName`),
  KEY `IX_Blobs_Time` (`Time`)
);

-- ----------------------------
-- Table structure for coupons
-- ----------------------------
DROP TABLE IF EXISTS `Coupons`;
CREATE TABLE `Coupons` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Description` longtext,
  `ImageId` bigint(20) NOT NULL,
  `MerchantId` bigint(20) NOT NULL,
  `Provider` longtext,
  `Time` int(11) NOT NULL,
  `Title` longtext,
  PRIMARY KEY (`Id`),
  KEY `IX_Coupons_MerchantId` (`MerchantId`)
);

-- ----------------------------
-- Table structure for paylogs
-- ----------------------------
DROP TABLE IF EXISTS `PayLogs`;
CREATE TABLE `PayLogs` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Balance` double NOT NULL,
  `MerchantId` bigint(20) NOT NULL,
  `Price` double NOT NULL,
  `Time` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PayLogs_MerchantId` (`MerchantId`),
  KEY `IX_PayLogs_Price` (`Price`),
  KEY `IX_PayLogs_Time` (`Time`)
);

-- ----------------------------
-- Table structure for redpockets
-- ----------------------------
DROP TABLE IF EXISTS `RedPockets`;
CREATE TABLE `RedPockets` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `ActivityId` bigint(20) NOT NULL,
  `AvatarUrl` varchar(256) DEFAULT NULL,
  `CouponId` bigint(20) DEFAULT NULL,
  `Ip` varchar(64) DEFAULT NULL,
  `NickName` varchar(64) DEFAULT NULL,
  `OpenId` varchar(32) DEFAULT NULL,
  `Price` bigint(20) NOT NULL,
  `ReceivedTime` datetime DEFAULT NULL,
  `Type` int(11) NOT NULL,
  `Url` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_RedPockets_ActivityId` (`ActivityId`),
  KEY `IX_RedPockets_Price` (`Price`),
  KEY `IX_RedPockets_ReceivedTime` (`ReceivedTime`),
  KEY `IX_RedPockets_Type` (`Type`)
);

-- ----------------------------
-- Table structure for templates
-- ----------------------------
DROP TABLE IF EXISTS `Templates`;
CREATE TABLE `Templates` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `BackgroundId` bigint(20) DEFAULT NULL,
  `BottomPartId` bigint(20) DEFAULT NULL,
  `DrawnId` bigint(20) DEFAULT NULL,
  `MerchantId` bigint(20) NOT NULL,
  `PendingId` bigint(20) DEFAULT NULL,
  `RuleUrl` varchar(64) DEFAULT NULL,
  `TopPartId` bigint(20) DEFAULT NULL,
  `Type` int(11) NOT NULL,
  `UndrawnId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
);

-- ----------------------------
-- Table structure for wallets
-- ----------------------------
DROP TABLE IF EXISTS `Wallets`;
CREATE TABLE `Wallets` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CouponId` bigint(20) NOT NULL,
  `Expire` datetime NOT NULL,
  `MerchantId` bigint(20) NOT NULL,
  `OpenId` varchar(32) DEFAULT NULL,
  `Time` datetime NOT NULL,
  `VerifyCode` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Wallets_MerchantId` (`MerchantId`),
  KEY `IX_Wallets_OpenId` (`OpenId`)
);
