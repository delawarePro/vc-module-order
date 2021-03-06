﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using VirtoCommerce.Domain.Commerce.Model;
using VirtoCommerce.Domain.Order.Model;
using VirtoCommerce.Domain.Payment.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrderModule.Data.Model
{
    public class PaymentInEntity : OperationEntity
    {
        [StringLength(64)]
        public string OrganizationId { get; set; }
        [StringLength(255)]
        public string OrganizationName { get; set; }

        [Required]
        [StringLength(64)]
        public string CustomerId { get; set; }
        [StringLength(255)]
        public string CustomerName { get; set; }

        public DateTime? IncomingDate { get; set; }
        [StringLength(128)]
        public string OuterId { get; set; }
        [StringLength(1024)]
        public string Purpose { get; set; }
        [StringLength(64)]
        public string GatewayCode { get; set; }

        public DateTime? AuthorizedDate { get; set; }
        public DateTime? CapturedDate { get; set; }
        public DateTime? VoidedDate { get; set; }

        [StringLength(64)]
        public string TaxType { get; set; }

        [Column(TypeName = "Money")]
        public decimal Price { get; set; }
        [Column(TypeName = "Money")]
        public decimal PriceWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal DiscountAmount { get; set; }
        [Column(TypeName = "Money")]
        public decimal DiscountAmountWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal Total { get; set; }
        [Column(TypeName = "Money")]
        public decimal TotalWithTax { get; set; }

        [Column(TypeName = "Money")]
        public decimal TaxTotal { get; set; }
        public decimal TaxPercentRate { get; set; }

        public virtual ObservableCollection<AddressEntity> Addresses { get; set; } = new NullCollection<AddressEntity>();
        public virtual ObservableCollection<PaymentGatewayTransactionEntity> Transactions { get; set; } = new NullCollection<PaymentGatewayTransactionEntity>();

        public string CustomerOrderId { get; set; }
        public virtual CustomerOrderEntity CustomerOrder { get; set; }

        public string ShipmentId { get; set; }
        public virtual ShipmentEntity Shipment { get; set; }

        public virtual ObservableCollection<DiscountEntity> Discounts { get; set; } = new NullCollection<DiscountEntity>();
        public virtual ObservableCollection<TaxDetailEntity> TaxDetails { get; set; } = new NullCollection<TaxDetailEntity>();


        public override OrderOperation ToModel(OrderOperation operation)
        {
            var payment = operation as PaymentIn;
            if (payment == null)
                throw new ArgumentException(@"operation argument must be of type PaymentIn", nameof(operation));

            if (!Addresses.IsNullOrEmpty())
            {
                payment.BillingAddress = Addresses.First().ToModel(AbstractTypeFactory<Address>.TryCreateInstance());
            }

            payment.Transactions = Transactions.Select(x => x.ToModel(AbstractTypeFactory<PaymentGatewayTransaction>.TryCreateInstance())).ToList();
            payment.TaxDetails = TaxDetails.Select(x => x.ToModel(AbstractTypeFactory<TaxDetail>.TryCreateInstance())).ToList();
            payment.Discounts = Discounts.Select(x => x.ToModel(AbstractTypeFactory<Discount>.TryCreateInstance())).ToList();

            base.ToModel(payment);

            payment.PaymentStatus = EnumUtility.SafeParse(Status, PaymentStatus.Custom);

            return payment;
        }

        public override OperationEntity FromModel(OrderOperation operation, PrimaryKeyResolvingMap pkMap)
        {
            var payment = operation as PaymentIn;
            if (payment == null)
                throw new ArgumentException(@"operation argument must be of type PaymentIn", nameof(operation));

            base.FromModel(payment, pkMap);

            Status = payment.PaymentStatus.ToString();

            if (payment.PaymentMethod != null)
            {
                GatewayCode = payment.PaymentMethod != null ? payment.PaymentMethod.Code : payment.GatewayCode;
            }

            if (payment.BillingAddress != null)
            {
                Addresses = new ObservableCollection<AddressEntity>(new[] { AbstractTypeFactory<AddressEntity>.TryCreateInstance().FromModel(payment.BillingAddress) });
            }

            if (payment.TaxDetails != null)
            {
                TaxDetails = new ObservableCollection<TaxDetailEntity>(payment.TaxDetails.Select(x => AbstractTypeFactory<TaxDetailEntity>.TryCreateInstance().FromModel(x)));
            }

            if (payment.Discounts != null)
            {
                Discounts = new ObservableCollection<DiscountEntity>(payment.Discounts.Select(x => AbstractTypeFactory<DiscountEntity>.TryCreateInstance().FromModel(x)));
            }

            if (payment.Transactions != null)
            {
                Transactions = new ObservableCollection<PaymentGatewayTransactionEntity>(payment.Transactions.Select(x => AbstractTypeFactory<PaymentGatewayTransactionEntity>.TryCreateInstance().FromModel(x, pkMap)));
            }

            return this;
        }

        public override void Patch(OperationEntity operation)
        {
            base.Patch(operation);

            var target = operation as PaymentInEntity;
            if (target == null)
                throw new ArgumentException(@"operation argument must be of type PaymentInEntity", nameof(operation));

            target.Price = Price;
            target.PriceWithTax = PriceWithTax;
            target.DiscountAmount = DiscountAmount;
            target.DiscountAmountWithTax = DiscountAmountWithTax;
            target.TaxType = TaxType;
            target.TaxPercentRate = TaxPercentRate;
            target.TaxTotal = TaxTotal;
            target.Total = Total;
            target.TotalWithTax = TotalWithTax;

            target.CustomerId = CustomerId;
            target.CustomerName = CustomerName;
            target.OrganizationId = OrganizationId;
            target.OrganizationName = OrganizationName;
            target.GatewayCode = GatewayCode;
            target.Purpose = Purpose;
            target.OuterId = OuterId;
            target.Status = Status;
            target.AuthorizedDate = AuthorizedDate;
            target.CapturedDate = CapturedDate;
            target.VoidedDate = VoidedDate;
            target.IsCancelled = IsCancelled;
            target.CancelledDate = CancelledDate;
            target.CancelReason = CancelReason;
            target.Sum = Sum;

            if (!Addresses.IsNullCollection())
            {
                Addresses.Patch(target.Addresses, new AddressComparer(), (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
            }

            if (!TaxDetails.IsNullCollection())
            {
                var taxDetailComparer = AnonymousComparer.Create((TaxDetailEntity x) => x.Name);
                TaxDetails.Patch(target.TaxDetails, taxDetailComparer, (sourceTaxDetail, targetTaxDetail) => sourceTaxDetail.Patch(targetTaxDetail));
            }

            if (!Discounts.IsNullCollection())
            {
                var discountComparer = AnonymousComparer.Create((DiscountEntity x) => x.PromotionId);
                Discounts.Patch(target.Discounts, discountComparer, (sourceDiscount, targetDiscount) => sourceDiscount.Patch(targetDiscount));
            }

            if (!Transactions.IsNullCollection())
            {
                Transactions.Patch(target.Transactions, (sourceTran, targetTran) => sourceTran.Patch(targetTran));
            }
        }
    }
}
