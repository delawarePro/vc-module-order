﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrderModule.Data.Model
{
    public class PaymentGatewayTransactionEntity : AuditableEntity
    {
        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }
        [StringLength(3)]
        public string Currency { get; set; }

        public bool IsProcessed { get; set; }
        public DateTime? ProcessedDate { get; set; }
        [StringLength(2048)]
        public string ProcessError { get; set; }
        public int ProcessAttemptCount { get; set; }

        public string RequestData { get; set; }

        public string ResponseData { get; set; }

        [StringLength(64)]
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gateway IP address
        /// </summary>
        [StringLength(128)]
        public string GatewayIpAddress { get; set; }

        [StringLength(64)]
        public string Type { get; set; }

        [StringLength(64)]
        public string Status { get; set; }

        [StringLength(2048)]
        public string Note { get; set; }

        public virtual PaymentInEntity PaymentIn { get; set; }
        public string PaymentInId { get; set; }


        public virtual PaymentGatewayTransaction ToModel(PaymentGatewayTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            transaction.Id = Id;
            transaction.Amount = Amount;
            transaction.CreatedBy = CreatedBy;
            transaction.CreatedDate = CreatedDate;
            transaction.CurrencyCode = Currency;
            transaction.GatewayIpAddress = GatewayIpAddress;
            transaction.IsProcessed = IsProcessed;
            transaction.ModifiedBy = ModifiedBy;
            transaction.ModifiedDate = ModifiedDate;
            transaction.Note = Note;
            transaction.ProcessAttemptCount = ProcessAttemptCount;
            transaction.ProcessedDate = ProcessedDate;
            transaction.ProcessError = ProcessError;
            transaction.RequestData = RequestData;
            transaction.ResponseCode = ResponseCode;
            transaction.ResponseData = ResponseData;
            transaction.Status = Status;
            transaction.Type = Type;

            return transaction;
        }

        public virtual PaymentGatewayTransactionEntity FromModel(PaymentGatewayTransaction transaction, PrimaryKeyResolvingMap pkMap)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            pkMap.AddPair(transaction, this);

            Id = transaction.Id;
            Amount = transaction.Amount;
            CreatedBy = transaction.CreatedBy;
            CreatedDate = transaction.CreatedDate;
            Currency = transaction.CurrencyCode;
            GatewayIpAddress = transaction.GatewayIpAddress;
            IsProcessed = transaction.IsProcessed;
            ModifiedBy = transaction.ModifiedBy;
            ModifiedDate = transaction.ModifiedDate;
            Note = transaction.Note;
            ProcessAttemptCount = transaction.ProcessAttemptCount;
            ProcessedDate = transaction.ProcessedDate;
            ProcessError = transaction.ProcessError;
            RequestData = transaction.RequestData;
            ResponseCode = transaction.ResponseCode;
            ResponseData = transaction.ResponseData;
            Status = transaction.Status;
            Type = transaction.Type;

            return this;
        }

        public virtual void Patch(PaymentGatewayTransactionEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.Amount = Amount;

            target.Currency = Currency;
            target.GatewayIpAddress = GatewayIpAddress;
            target.IsProcessed = IsProcessed;

            target.Note = Note;
            target.ProcessAttemptCount = ProcessAttemptCount;
            target.ProcessedDate = ProcessedDate;
            target.ProcessError = ProcessError;
            target.RequestData = RequestData;
            target.ResponseCode = ResponseCode;
            target.ResponseData = ResponseData;
            target.Status = Status;
            target.Type = Type;
        }
    }
}
