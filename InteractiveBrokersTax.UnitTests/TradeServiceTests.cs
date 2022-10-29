// using System;
// using System.Collections.Generic;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using UnitTest.Common;
//
// namespace Investing.Common.UnitTests
// {
//     [TestClass]
//     public class TradeServiceTests
//     {
//         [TestMethod]
//         public void Separate_NotChanged()
//         {
//             var trade1 = new Trade()
//             {
//                 Quantity = 2,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -20
//             };
//             var trade2 = new Trade()
//             {
//                 Quantity = -2,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = 20
//             };
//             var trade3 = new Trade()
//             {
//                 Quantity = 2,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -20
//             };
//             var trade4 = new Trade()
//             {
//                 Quantity = -2,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = 20
//             };
//             
//             var trades = new List<Trade>() {trade1, trade2, trade3, trade4};
//
//             var actual = TradeService.Separate(trades);
//
//             ContentAssert.AreEqual(trade1, actual[0]);
//             ContentAssert.AreEqual(trade2, actual[1]);
//             ContentAssert.AreEqual(trade3, actual[2]);
//             ContentAssert.AreEqual(trade4, actual[3]);
//         }     
//         
//         [TestMethod]
//         public void Separate_OneClosedPosition2()
//         {
//             var trade1 = new Trade()
//             {
//                 Quantity = 1,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -10
//             };         
//             var trade2 = new Trade()
//             {
//                 Quantity = 3,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -30
//             };
//             var trade3 = new Trade()
//             {
//                 Quantity = -1,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = 10
//             };
//             
//             var trades = new List<Trade>() {trade1, trade2, trade3};
//
//             var actual = TradeService.Separate(trades);
//
//             ContentAssert.AreEqual(trade1, actual[0]);
//             ContentAssert.AreEqual(trade3, actual[1]);
//             ContentAssert.AreEqual(trade2, actual[2]);
//         }     
//         
//         [TestMethod]
//         public void Separate_OneClosedPosition3()
//         {
//             var trade1 = new Trade()
//             {
//                 Quantity = 1,
//                 TransactionPrice = 10,
//                 Gain = -10
//             };         
//             var trade2 = new Trade()
//             {
//                 Quantity = 2,
//                 TransactionPrice = 10,
//                 Gain = -20
//             };
//             var trade3 = new Trade()
//             {
//                 Quantity = 4,
//                 TransactionPrice = 10,
//                 Gain = -40
//             };
//             var trade4 = new Trade()
//             {
//                 Quantity = 5,
//                 TransactionPrice = 10,
//                 Gain = -50
//             };
//             var trade5 = new Trade()
//             {
//                 Quantity = -6,
//                 TransactionPrice = 10,
//                 Gain = 60
//             };
//             
//             var trades = new List<Trade>() {trade1, trade2, trade3, trade4, trade5};
//
//             var actual = TradeService.Separate(trades);
//
//             var expectedTrade1 = new Trade()
//             {
//                 Quantity = 1,
//                 TransactionPrice = 10,
//                 Gain = -10
//             };         
//             var expectedTrade2 = new Trade()
//             {
//                 Quantity = 2,
//                 TransactionPrice = 10,
//                 Gain = -20
//             };
//             var expectedTrade3 = new Trade()
//             {
//                 Quantity = 3,
//                 TransactionPrice = 10,
//                 Gain = -30
//             };
//             var expectedTrade4 = new Trade()
//             {
//                 Quantity = -6,
//                 TransactionPrice = 10,
//                 Gain = 60
//             };
//             var expectedTrade5 = new Trade()
//             {
//                 Quantity = 1,
//                 TransactionPrice = 10,
//                 Gain = -10
//             };
//             var expectedTrade6 = new Trade()
//             {
//                 Quantity = 5,
//                 TransactionPrice = 10,
//                 Gain = -50
//             };
//             
//             ContentAssert.AreEqual(expectedTrade1, actual[0]);
//             ContentAssert.AreEqual(expectedTrade2, actual[1]);
//             ContentAssert.AreEqual(expectedTrade3, actual[2]);
//             ContentAssert.AreEqual(expectedTrade4, actual[3]);
//             ContentAssert.AreEqual(expectedTrade5, actual[4]);
//             ContentAssert.AreEqual(expectedTrade6, actual[5]);
//         }  
//         
//         [TestMethod]
//         public void Separate_OneClosedPosition()
//         {
//             var trade1 = new Trade()
//             {
//                 Quantity = 1,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -10
//             };
//             var trade2 = new Trade()
//             {
//                 Quantity = 6,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -60
//             };
//             var trade3 = new Trade()
//             {
//                 Quantity = -3,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = 30
//             };
//             var trades = new List<Trade>() {trade1, trade2, trade3};
//
//             var actual = TradeService.Separate(trades);
//
//             var expectedTrade1 = new Trade()
//             {
//                 Quantity = 1,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -10
//             };
//             var expectedTrade2 = new Trade()
//             {
//                 Quantity = 2,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -20
//             };
//             var expectedTrade3 = new Trade()
//             {
//                 Quantity = -3,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = 30
//             };
//             var expectedTrade4 = new Trade()
//             {
//                 Quantity = 4,
//                 Comission = 0,
//                 TransactionPrice = 10,
//                 Gain = -40
//             };
//             
//             ContentAssert.AreEqual(expectedTrade1, actual[0]);
//             ContentAssert.AreEqual(expectedTrade2, actual[1]);
//             ContentAssert.AreEqual(expectedTrade3, actual[2]);
//             ContentAssert.AreEqual(expectedTrade4, actual[3]);
//         }    
//         
//         [TestMethod]
//         public void Separate_TwoClosedPositions()
//         {
//             var trade1 = new Trade()
//             {
//                 Quantity = 1,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -10
//             };
//             var trade2 = new Trade()
//             {
//                 Quantity = 3,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -30
//             };
//             var trade3 = new Trade()
//             {
//                 Quantity = -2,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = 20
//             };
//             var trade4 = new Trade()
//             {
//                 Quantity = -1,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = 10
//             };
//             var trades = new List<Trade>() {trade1, trade2, trade3, trade4};
//
//             var actual = TradeService.Separate(trades);
//
//             var expectedTrade1 = new Trade()
//             {
//                 Quantity = 1,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -10
//             };
//             var expectedTrade2 = new Trade()
//             {
//                 Quantity = 1,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = -10
//             };
//             var expectedTrade3 = new Trade()
//             {
//                 Quantity = -2,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = 20
//             };
//             var expectedTrade4 = new Trade()
//             {
//                 Quantity = 1,
//                 Comission = 0,
//                 TransactionPrice = 10,
//                 Gain = -10
//             };       
//             var expectedTrade5 = new Trade()
//             {
//                 Quantity = -1,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 Gain = 10
//             };
//             var expectedTrade6 = new Trade()
//             {
//                 Quantity = 1,
//                 Comission = 0,
//                 TransactionPrice = 10,
//                 Gain = -10
//             };
//             
//             ContentAssert.AreEqual(expectedTrade1, actual[0]);
//             ContentAssert.AreEqual(expectedTrade2, actual[1]);
//             ContentAssert.AreEqual(expectedTrade3, actual[2]);
//             ContentAssert.AreEqual(expectedTrade4, actual[3]);
//             ContentAssert.AreEqual(expectedTrade5, actual[4]);
//             ContentAssert.AreEqual(expectedTrade6, actual[5]);
//         }
//
//         [TestMethod]
//         public void Split()
//         {
//             var trade1 = new Trade()
//             {
//                 Quantity = 2,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 DateTime = new DateTime(2021, 01, 10),
//                 Gain = -20
//             };
//             var trade2 = new Trade()
//             {
//                 Quantity = 2,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 DateTime = new DateTime(2021, 01, 20),
//                 Gain = -20
//             };       
//             var trade3 = new Trade()
//             {
//                 Quantity = -2,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 DateTime = new DateTime(2021, 01, 20),
//                 Gain = 20
//             };
//             var trades = new List<Trade>() {trade1, trade2, trade3};
//             
//             var actual = TradeService.Split(trades, new DateTime(2021, 01, 15), 5);
//             
//             var expectedTrade1 = new Trade()
//             {
//                 Quantity = 2,
//                 Comission = -1,
//                 TransactionPrice = 10,
//                 DateTime = new DateTime(2021, 01, 10),
//                 Gain = -20
//             };
//             var expectedTrade2 = new Trade()
//             {
//                 Quantity = 10,
//                 Comission = -1,
//                 TransactionPrice = 2,
//                 DateTime = new DateTime(2021, 01, 20),
//                 Gain = -20
//             };     
//             var expectedTrade3 = new Trade()
//             {
//                 Quantity = -10,
//                 Comission = -1,
//                 TransactionPrice = 2,
//                 DateTime = new DateTime(2021, 01, 20),
//                 Gain = 20
//             };
//             
//             ContentAssert.AreEqual(expectedTrade1, actual[0]);
//             ContentAssert.AreEqual(expectedTrade2, actual[1]);
//             ContentAssert.AreEqual(expectedTrade3, actual[2]);
//         }
//     }
// }