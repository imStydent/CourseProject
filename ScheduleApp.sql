CREATE DATABASE  IF NOT EXISTS `schedulebd` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `schedulebd`;
-- MySQL dump 10.13  Distrib 8.0.41, for Win64 (x86_64)
--
-- Host: localhost    Database: schedulebd
-- ------------------------------------------------------
-- Server version	8.0.19

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `load_unload_operations`
--

DROP TABLE IF EXISTS `load_unload_operations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `load_unload_operations` (
  `id` int NOT NULL AUTO_INCREMENT,
  `date` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `load_unload_operations`
--

LOCK TABLES `load_unload_operations` WRITE;
/*!40000 ALTER TABLE `load_unload_operations` DISABLE KEYS */;
INSERT INTO `load_unload_operations` VALUES (1,'2025-12-11 00:00:00'),(2,'2025-12-08 00:00:00'),(3,'2025-12-09 00:00:00'),(4,'2025-12-10 00:00:00'),(5,'2025-12-12 00:00:00'),(6,'2025-12-13 00:00:00'),(7,'2025-12-14 00:00:00'),(8,'2025-12-15 00:00:00'),(9,'2025-12-16 00:00:00'),(10,'2025-12-17 00:00:00'),(11,'2025-12-18 00:00:00'),(12,'2025-12-19 00:00:00'),(13,'2025-12-20 00:00:00'),(14,'2025-12-21 00:00:00');
/*!40000 ALTER TABLE `load_unload_operations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `load_time` time NOT NULL,
  `unload_time` time NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=60 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (4,'Заказ 4','09:45:00','10:45:00'),(5,'Заказ 5','11:15:00','12:15:00'),(20,'Заказ 1','10:00:00','11:00:00'),(21,'Заказ 1','10:00:00','11:00:00'),(36,'Магнит','01:00:00','03:00:00'),(37,'Магнит','01:00:00','03:00:00'),(38,'Пятерочка','03:00:00','04:00:00'),(39,'Макси','05:00:00','06:00:00'),(40,'Пятерочка','12:00:00','13:00:00'),(41,'Магнит','10:00:00','11:00:00'),(42,'Макси','12:00:00','15:00:00'),(43,'Макси','01:00:00','02:00:00'),(44,'Пятерочка','01:00:00','03:00:00'),(45,'Магнит','13:00:00','14:00:00'),(46,'Магнит','13:00:00','15:00:00'),(47,'Пятерочка','03:00:00','05:00:00'),(48,'Макси','07:00:00','09:00:00'),(49,'ЮГ - 4','01:00:00','03:00:00'),(50,'АГРОТОРГ КДК, Ленина','05:00:00','06:00:00'),(51,'Лента','04:00:00','06:00:00'),(52,'Магнит','07:00:00','08:00:00'),(53,'Холмогоры','03:00:00','05:00:00'),(54,'ТРОИЦКИЙ','05:00:00','06:00:00'),(55,'Макси','00:00:00','05:00:00'),(56,'Магнит','05:00:00','07:00:00'),(57,'Пятерочка','09:00:00','12:00:00'),(58,'Макси','04:00:00','06:00:00'),(59,'СЕВМАШ','05:00:00','10:00:00');
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders_has_load_unload_opertations`
--

DROP TABLE IF EXISTS `orders_has_load_unload_opertations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders_has_load_unload_opertations` (
  `Orders_id` int NOT NULL,
  `load_unload_opertations_id` int NOT NULL,
  PRIMARY KEY (`Orders_id`,`load_unload_opertations_id`),
  KEY `fk_Orders_has_load_unload_opertations_Orders1_idx` (`Orders_id`),
  KEY `fk_Orders_has_load_unload_opertations_load_unload_opertatio_idx` (`load_unload_opertations_id`),
  CONSTRAINT `fk_Orders_has_load_unload_opertations_load_unload_opertations1` FOREIGN KEY (`load_unload_opertations_id`) REFERENCES `load_unload_operations` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_Orders_has_load_unload_opertations_Orders1` FOREIGN KEY (`Orders_id`) REFERENCES `orders` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders_has_load_unload_opertations`
--

LOCK TABLES `orders_has_load_unload_opertations` WRITE;
/*!40000 ALTER TABLE `orders_has_load_unload_opertations` DISABLE KEYS */;
INSERT INTO `orders_has_load_unload_opertations` VALUES (36,1),(37,2),(38,2),(39,2),(40,3),(41,3),(42,1),(43,4),(44,5),(45,5),(46,6),(47,7),(48,7),(49,8),(50,8),(51,9),(52,9),(53,10),(54,11),(55,12),(56,12),(57,12),(58,13),(59,14);
/*!40000 ALTER TABLE `orders_has_load_unload_opertations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `kind` enum('Термоупаковка','Ящик тетра-крейт','Село Устьяны','Радостино') NOT NULL,
  `pieces_per_box` tinyint NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,'Молоко 2.5','Термоупаковка',10),(2,'Кефир 2.5% ','Ящик тетра-крейт',5),(3,'У-Кефир ОТБОРНЫЙ','Село Устьяны',12),(4,'Р-СЫРОК ИЗЮМ 6% 0.100 кг','Радостино',8),(5,'Кефир Яблоко','Термоупаковка',15),(6,'Сметана 0.45 кг','Термоупаковка',12),(7,'Творог 0,5 кг','Ящик тетра-крейт',15),(8,'Десерт ванильный','Ящик тетра-крейт',10),(9,'РЯЖЕНКА ОТБОРНАЯ','Термоупаковка',4),(10,'ЙОГУРТ Белый Персик','Термоупаковка',5);
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products_has_orders`
--

DROP TABLE IF EXISTS `products_has_orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products_has_orders` (
  `Products_id` int NOT NULL,
  `Orders_id` int NOT NULL,
  `amount` smallint NOT NULL,
  PRIMARY KEY (`Products_id`,`Orders_id`),
  KEY `fk_Products_has_Orders_Orders1_idx` (`Orders_id`),
  KEY `fk_Products_has_Orders_Products1_idx` (`Products_id`),
  CONSTRAINT `fk_Products_has_Orders_Orders1` FOREIGN KEY (`Orders_id`) REFERENCES `orders` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_Products_has_Orders_Products1` FOREIGN KEY (`Products_id`) REFERENCES `products` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products_has_orders`
--

LOCK TABLES `products_has_orders` WRITE;
/*!40000 ALTER TABLE `products_has_orders` DISABLE KEYS */;
INSERT INTO `products_has_orders` VALUES (1,20,12),(1,21,12),(1,36,10),(1,37,8),(1,41,5),(1,42,5),(1,44,7),(1,45,7),(1,47,10),(1,48,6),(1,49,5),(1,51,8),(1,53,8),(1,54,4),(1,57,6),(1,58,5),(1,59,7),(2,20,5),(2,21,5),(2,36,10),(2,37,12),(2,39,12),(2,40,2),(2,43,5),(2,46,6),(2,50,12),(2,52,5),(2,55,6),(2,56,6),(3,20,4),(3,49,6),(3,51,3),(4,21,2),(4,45,3),(4,46,4),(5,37,5),(5,38,5),(5,39,5),(5,42,5),(5,44,3),(5,49,5),(6,36,5),(6,46,3),(6,48,6),(6,51,5),(6,52,5),(6,54,4),(6,55,10),(6,56,4),(6,58,5),(6,59,6),(7,36,5),(7,37,5),(7,39,12),(7,40,2),(7,48,4),(7,50,12),(7,51,4),(7,52,1),(7,55,5),(7,56,5),(7,57,5),(7,58,5),(7,59,6),(8,39,3),(8,48,5),(8,50,2),(8,52,5),(8,54,5),(9,42,12),(10,43,4),(10,44,4),(10,57,6);
/*!40000 ALTER TABLE `products_has_orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'Работник'),(2,'Менеджер'),(3,'Администратор');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `workers`
--

DROP TABLE IF EXISTS `workers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `workers` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(60) NOT NULL,
  `surname` varchar(60) NOT NULL,
  `patronymic` varchar(60) NOT NULL,
  `phone_number` varchar(100) NOT NULL,
  `role_id` int NOT NULL,
  `password` varchar(8) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_Workers_Roles_idx` (`role_id`),
  CONSTRAINT `fk_Workers_Roles` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `workers`
--

LOCK TABLES `workers` WRITE;
/*!40000 ALTER TABLE `workers` DISABLE KEYS */;
INSERT INTO `workers` VALUES (2,'Томас','Андерсон','Вадимович','89600075544',2,'54321'),(4,'Артём','Мошников','Вячеславович','89600043011',3,'12345'),(6,'Вадим','Мошников','Александрович','89600075566',1,'1234567');
/*!40000 ALTER TABLE `workers` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-12-10 23:44:57
