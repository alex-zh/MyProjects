﻿using System;
using Robot.Trans2Quik.Components;
using Robot.Trans2Quik.Entities;
using Robot.Trans2Quik.Entities.Transaction;
using Robot.Trans2Quik.Entities.Transaction.Order;
using Robot.Trans2Quik.Internals;
using Trans2Quik.Core;
using Trans2Quik.Core.Components;

namespace Robot.Trans2Quik
{
    public class Gateway
    {
        private readonly ConnectionListener connectionListener;
        private readonly OrderInfoListener orderInfoListener;
        private readonly TradeInfoListener tradeInfoListener;
        private readonly TransactionManager transactionManager;
        private readonly TransactionBuilder transactionBuilder;

        protected string ClassCode { get; private set; }
        protected string SecurityCode { get; private set; }

        public bool IsConnected
        {
            get
            {
                return connectionListener.IsConnected;
            }
        }

        public bool IsDllConnected
        {
            get
            {
                return connectionListener.IsDllConnected;
            }
        }

        public bool IsQuikConnected
        {
            get
            {
                return connectionListener.IsQuikConnected;
            }
        }

        public event EventHandler<OrderInfoEventArgs> OrderChanged;
        public event EventHandler<TradeInfoEventArgs> TradeChanged;
        public event EventHandler<TransactionEventArgs> NewTransaction;
        public event EventHandler ConnectionStatusChanged;

        public Gateway(string pathToQuik, string account)
        {
            this.connectionListener = new ConnectionListener(pathToQuik);
            this.orderInfoListener = new OrderInfoListener();
            this.tradeInfoListener = new TradeInfoListener();
            this.transactionManager = new TransactionManager(false);
            this.transactionBuilder = new TransactionBuilder(account);

            this.connectionListener.ConnectionStatusChanged += ConnectionStatusChangedHandler;
            this.orderInfoListener.OrderStatusChanged += OnOrderChanged;
            this.tradeInfoListener.TradeStatusChanged += OnTradeChanged;
            this.transactionManager.TransactionAsyncReply += TransactionAsyncReply;
        }

        public bool SendOrder(string classCode, string secCode, Direction direction, int quantity, decimal price = decimal.Zero)
        {
            var tradeParams = new OrderTradeParams(new Security(classCode, secCode), direction, quantity, price);
            return this.SendOrder(tradeParams);
        }

        public bool SendOrder(OrderTradeParams tradeParams)
        {
            var txn = this.transactionBuilder.NewOrder(tradeParams);
            return this.SendTransaction(txn.ToString());
        }

        public TransactionCallResult SendOrderSync(OrderTradeParams tradeParams)
        {
            var txn = this.transactionBuilder.NewOrder(tradeParams);
            return this.SendTransactionSync(txn.ToString());
        }

        public bool SendStopLimitOrder(StopOrderTradeParams tradeParams)
        {
            var txn = this.transactionBuilder.NewStopLimitOrder(tradeParams);
            return this.SendTransaction(txn.ToString());
        }

        public bool SendTakeProfitOrder(StopOrderTradeParams tradeParams)
        {
            var txn = this.transactionBuilder.NewTakeProfitOrder(tradeParams);
            return this.SendTransaction(txn.ToString());
        }

        public bool SendTakeProfitAndStopLimitOrder(StopOrderTradeParams tradeParams)
        {
            var txn = this.transactionBuilder.NewTakeProfitAndStopLimitOrder(tradeParams);
            return this.SendTransaction(txn.ToString());
        }

        public TransactionCallResult KillOrderSync(Security security, string orderKey)
        {
            var txn = transactionBuilder.KillOrder(security, orderKey);
            return SendTransactionSync(txn.ToString());
        }

        public bool KillOrder(Security security, string orderKey)
        {
            var txn = this.transactionBuilder.KillOrder(security, orderKey);
            return this.SendTransaction(txn.ToString());
        }

        public bool KillStopOrder(Security security, string orderKey)
        {
            var txn = this.transactionBuilder.KillStopOrder(security, orderKey);
            return this.SendTransaction(txn.ToString());
        }

        public bool Connect()
        {
            if (IsConnected) return true;

            return connectionListener.Connect();
        }

        public bool Disconnect()
        {
            if (IsConnected == false) return true;

            return connectionListener.Disconnect();
        }

        public bool Start(string classCode = "", string securityCode = "")
        {
            this.ClassCode = classCode;
            this.SecurityCode = securityCode;

            if (!this.IsConnected)
            {
                if (!this.connectionListener.Connect())
                {
                    return false;
                }
            }

            return this.Subscribe(this.ClassCode, this.SecurityCode) && this.StartWatch();
            //return this.StartWatch();
        }

        public void Stop()
        {
            this.orderInfoListener.Unsubscribe();
            this.tradeInfoListener.Unsubscribe();

            this.connectionListener.Disconnect();
        }

        public bool SendTransaction(string transactionString)
        {
            return this.transactionManager.SendAsyncTransaction(transactionString);
        }

        public TransactionCallResult SendTransactionSync(string transactionString)
        {
            return this.transactionManager.SendSyncTransaction(transactionString);
        }

        private bool Subscribe(string classCode, string securityCode)
        {
            var ordersSubscribed = this.orderInfoListener.Subscribe(classCode, securityCode);
            var tradesSubscribed = this.tradeInfoListener.Subscribe(classCode, securityCode);
            return ordersSubscribed && tradesSubscribed;
        }

        private bool StartWatch()
        {
            var ordersStarted = this.orderInfoListener.StartOrders();
            var tradesStarted = this.tradeInfoListener.StartTrades();

            return ordersStarted && tradesStarted;
        }

        private void ConnectionStatusChangedHandler(object sender, EventArgs e)
        {
            if (ConnectionStatusChanged != null)
            {
                ConnectionStatusChanged(this, EventArgs.Empty);
            }
        }

        private void OnOrderChanged(object sender, OrderInfoEventArgs e)
        {
            var tmp = this.OrderChanged;

            if (tmp != null)
            {
                tmp(this, e);
            }
        }

        private void OnTradeChanged(object sender, TradeInfoEventArgs e)
        {
            var tmp = this.TradeChanged;

            if (tmp != null)
            {
                tmp(this, e);
            }
        }

        private void TransactionAsyncReply(object sender, TransactionEventArgs e)
        {
            var tmp = this.NewTransaction;

            if (tmp != null)
            {
                tmp(this, e);
            }
        }
    }
}
