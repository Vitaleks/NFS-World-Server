-- phpMyAdmin SQL Dump
-- version 3.4.11.1deb2
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Jan 25, 2016 at 12:52 PM
-- Server version: 5.5.35
-- PHP Version: 5.4.4-14+deb7u8
--
-- Copyright (c) 2015-2016 Vitaleks
-- Copyright (c) 2015 Edmundas919
-- Copyright (c) 2015 mc3dcm
-- 
-- This file is part of NFS World Server.
--
-- NFS World Server is free software: you can redistribute it and/or modify
-- it under the terms of the GNU General Public License as published by
-- the Free Software Foundation, either version 3 of the License, or
-- (at your option) any later version.
-- 
-- NFS World Server is distributed in the hope that it will be useful,
-- but WITHOUT ANY WARRANTY; without even the implied warranty of
-- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
-- GNU General Public License for more details.
--
-- You should have received a copy of the GNU General Public License
-- along with NFS World Server.  If not, see <http://www.gnu.org/licenses/>.
--

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `nfsworld_server`
--

-- --------------------------------------------------------

--
-- Table structure for table `EventSession`
--

CREATE TABLE IF NOT EXISTS `EventSession` (
  `sessionId` int(11) NOT NULL AUTO_INCREMENT,
  `trackId` int(11) NOT NULL,
  `sessionType` varchar(12) COLLATE utf8_bin NOT NULL,
  `challangeId` varchar(20) COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`sessionId`),
  UNIQUE KEY `sessionId` (`sessionId`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=534 ;

-- --------------------------------------------------------

--
-- Table structure for table `EventSessionResults`
--

CREATE TABLE IF NOT EXISTS `EventSessionResults` (
  `sessionId` int(11) DEFAULT NULL,
  `personaId` int(11) DEFAULT NULL,
  `alternateEventDurationInMilliseconds` int(11) DEFAULT NULL,
  `carId` int(11) DEFAULT NULL,
  `eventDurationInMilliseconds` int(11) DEFAULT NULL,
  `finishReason` smallint(6) DEFAULT NULL,
  `rank` int(11) DEFAULT NULL,
  `copsDeployed` int(11) DEFAULT NULL,
  `copsDisabled` int(11) DEFAULT NULL,
  `copsRammed` int(11) DEFAULT NULL,
  `costToState` int(11) DEFAULT NULL,
  `infractions` int(11) DEFAULT NULL,
  `longestJumpDurationInMilliseconds` int(11) DEFAULT NULL,
  `roadBlocksDodged` int(11) DEFAULT NULL,
  `spikeStripsDodged` int(11) DEFAULT NULL,
  `sumOfJumpsDurationInMilliseconds` int(11) DEFAULT NULL,
  `topSpeed` float DEFAULT NULL,
  `bestLapDurationInMilliseconds` int(11) DEFAULT NULL,
  `fractionCompleted` float DEFAULT NULL,
  `numberOfCollisions` int(11) DEFAULT NULL,
  `perfectStart` bit(1) DEFAULT NULL,
  `accelerationAverge` float DEFAULT NULL,
  `accelerationMaximum` float DEFAULT NULL,
  `accelerationMedian` float DEFAULT NULL,
  `speedAverage` float NOT NULL,
  `speedMaximum` float DEFAULT NULL,
  `speedMedian` float DEFAULT NULL,
  `finishTime` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `FriendList`
--

CREATE TABLE IF NOT EXISTS `FriendList` (
  `userId` int(11) NOT NULL,
  `friendUserId` int(11) NOT NULL,
  `friendStatus` tinyint(4) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `Persona`
--

CREATE TABLE IF NOT EXISTS `Persona` (
  `personaId` int(11) NOT NULL AUTO_INCREMENT,
  `userId` int(11) NOT NULL,
  `name` varchar(20) COLLATE utf8_bin NOT NULL DEFAULT 'N/A',
  `motto` varchar(200) COLLATE utf8_bin NOT NULL DEFAULT 'Private server V0.7',
  `cash` int(11) NOT NULL DEFAULT '2500000',
  `level` int(11) NOT NULL DEFAULT '2',
  `iconIndex` smallint(6) NOT NULL DEFAULT '1',
  `rep` int(11) NOT NULL DEFAULT '0',
  `score` int(11) NOT NULL DEFAULT '0',
  `defaultCarIndex` int(11) NOT NULL DEFAULT '0',
  `carCount` int(11) NOT NULL DEFAULT '0',
  `carriera` bit(1) NOT NULL DEFAULT b'0',
  UNIQUE KEY `personaId` (`personaId`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=100 ;

-- --------------------------------------------------------

--
-- Table structure for table `PersonaCar`
--

CREATE TABLE IF NOT EXISTS `PersonaCar` (
  `carId` int(11) NOT NULL AUTO_INCREMENT,
  `personaId` int(11) DEFAULT NULL,
  `baseCar` int(11) DEFAULT NULL,
  `carClassHash` int(11) DEFAULT NULL,
  `physicsProfileHash` int(11) DEFAULT NULL,
  `rating` smallint(6) DEFAULT NULL,
  `resalePrice` int(11) DEFAULT NULL,
  `durability` tinyint(4) DEFAULT NULL,
  `heat` tinyint(4) DEFAULT NULL,
  PRIMARY KEY (`carId`),
  UNIQUE KEY `carId` (`carId`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=100 ;

-- --------------------------------------------------------

--
-- Table structure for table `PersonaCarPaints`
--

CREATE TABLE IF NOT EXISTS `PersonaCarPaints` (
  `carId` int(11) NOT NULL,
  `sat` tinyint(4) unsigned NOT NULL,
  `slot` tinyint(4) unsigned NOT NULL,
  `var` tinyint(4) unsigned NOT NULL,
  `hue` int(11) NOT NULL,
  `grp` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `PersonaCarPerformanceParts`
--

CREATE TABLE IF NOT EXISTS `PersonaCarPerformanceParts` (
  `carId` int(11) NOT NULL,
  `slot` int(11) NOT NULL,
  `partHash` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `PersonaCarSkills`
--

CREATE TABLE IF NOT EXISTS `PersonaCarSkills` (
  `carId` int(11) NOT NULL,
  `slot` int(11) NOT NULL,
  `skillHash` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `PersonaCarVinyls`
--

CREATE TABLE IF NOT EXISTS `PersonaCarVinyls` (
  `carId` int(11) DEFAULT NULL,
  `layer` int(11) unsigned DEFAULT NULL,
  `vinylHash` int(11) DEFAULT NULL,
  `hue1` int(11) DEFAULT NULL,
  `hue2` int(11) DEFAULT NULL,
  `hue3` int(11) DEFAULT NULL,
  `hue4` int(11) DEFAULT NULL,
  `mir` bit(1) DEFAULT NULL,
  `rot` tinyint(3) unsigned DEFAULT NULL,
  `sat1` tinyint(3) unsigned DEFAULT NULL,
  `sat2` tinyint(3) unsigned DEFAULT NULL,
  `sat3` tinyint(3) unsigned DEFAULT NULL,
  `sat4` tinyint(3) unsigned DEFAULT NULL,
  `scaleX` smallint(6) unsigned DEFAULT NULL,
  `scaleY` smallint(6) unsigned DEFAULT NULL,
  `shear` tinyint(3) unsigned DEFAULT NULL,
  `tranX` smallint(6) DEFAULT NULL,
  `tranY` smallint(6) DEFAULT NULL,
  `var1` tinyint(4) unsigned DEFAULT NULL,
  `var2` tinyint(4) unsigned DEFAULT NULL,
  `var3` tinyint(4) unsigned DEFAULT NULL,
  `var4` tinyint(4) unsigned DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `PersonaCarVisualParts`
--

CREATE TABLE IF NOT EXISTS `PersonaCarVisualParts` (
  `carId` int(11) DEFAULT NULL,
  `partHash` int(11) DEFAULT NULL,
  `slotHash` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

-- --------------------------------------------------------

--
-- Table structure for table `Users`
--

CREATE TABLE IF NOT EXISTS `Users` (
  `userId` int(11) NOT NULL AUTO_INCREMENT,
  `userEmail` varchar(50) COLLATE utf8_bin NOT NULL DEFAULT 'email@email.email',
  `userPassword` varchar(32) COLLATE utf8_bin NOT NULL DEFAULT 'password',
  `userSession` varchar(36) COLLATE utf8_bin NOT NULL,
  `userSessionTime` int(10) NOT NULL,
  `boost` int(10) NOT NULL DEFAULT '10000',
  `personaNumber` tinyint(6) NOT NULL DEFAULT '0',
  `userLauncherSession` varchar(36) COLLATE utf8_bin NOT NULL,
  `banned` bit(1) NOT NULL DEFAULT b'0',
  `userSubscribe` int(10) NOT NULL DEFAULT '0',
  KEY `userId` (`userId`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COLLATE=utf8_bin AUTO_INCREMENT=1 ;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
