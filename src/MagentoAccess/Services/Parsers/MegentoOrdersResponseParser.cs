﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MagentoAccess.Misc;
using MagentoAccess.Models.GetOrders;

namespace MagentoAccess.Services.Parsers
{
	public class MegentoOrdersResponseParser : MagentoBaseResponseParser< Order >
	{
		public override Order Parse( Stream stream, bool keepStremPosition = true )
		{
			try
			{
				XNamespace ns = "urn:ebay:apis:eBLBaseComponents";

				var streamStartPos = stream.Position;

				var root = XElement.Load( stream );

				var orderDataItems = root.Descendants( ns + "data_item" );

				var error = this.ResponseContainsErrors( root, ns );

				if( error != null )
					return new GetOrdersResponse { Error = error };

				var orders = orderDataItems.Select( x =>
				{
					string temp;

					var resultOrder = new Order();

					resultOrder.OrderId = GetElementValue( x, ns, "entity_id" );

					//todo: to enum
					resultOrder.Status = GetElementValue( x, ns, "status" );

					resultOrder.Customer = GetElementValue( x, ns, "customer_id" );

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_discount_amount" ) ) )
						resultOrder.BaseDiscount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_grand_total" ) ) )
						resultOrder.BaseGrandTotal = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_shipping_amount" ) ) )
						resultOrder.BaseShippingAmount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_shipping_tax_amount" ) ) )
						resultOrder.BaseShippingTaxAmount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_subtotal" ) ) )
						resultOrder.BaseSubtotal = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_tax_amount" ) ) )
						resultOrder.BaseTaxAmount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_discount_amount" ) ) )
						resultOrder.BaseDiscount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_total_paid" ) ) )
						resultOrder.BaseTotalPaid = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_total_refunded" ) ) )
						resultOrder.BaseTotalRefunded = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "discount_amount" ) ) )
						resultOrder.DiscountAmount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "grand_total" ) ) )
						resultOrder.GrandTotal = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "shipping_amount" ) ) )
						resultOrder.ShippingAmount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "shipping_tax_amount" ) ) )
						resultOrder.ShippingTaxAmount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "store_to_order_rate" ) ) )
						resultOrder.StoreToOrderRate = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_discount_amount" ) ) )
						resultOrder.BaseDiscount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "subtotal" ) ) )
						resultOrder.Subtotal = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "tax_amount" ) ) )
						resultOrder.TaxAmount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "total_paid" ) ) )
						resultOrder.TotalPaid = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "total_refunded" ) ) )
						resultOrder.TotalRefunded = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_shipping_discount_amount" ) ) )
						resultOrder.BaseShippingDiscountAmount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_subtotal_incl_tax" ) ) )
						resultOrder.BaseSubtotalInclTax = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_total_due" ) ) )
						resultOrder.BaseTotalDue = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "base_discount_amount" ) ) )
						resultOrder.BaseDiscount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "shipping_discount_amount" ) ) )
						resultOrder.ShippingDiscountAmount = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "subtotal_incl_tax" ) ) )
						resultOrder.SubtotalInclTax = temp.ToDecimalDotOrComaSeparated();

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "total_due" ) ) )
						resultOrder.TotalDue = temp.ToDecimalDotOrComaSeparated();

					//todo: to enum
					resultOrder.BaseCurrencyCode = GetElementValue( x, ns, "base_currency_code" );

					resultOrder.StoreName = GetElementValue( x, ns, "store_name" );

					resultOrder.CreatedAt = GetElementValue( x, ns, "created_at" );

					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "shipping_incl_tax" ) ) )
						resultOrder.ShippingInclTax = temp.ToDecimalDotOrComaSeparated();

					//todo: to enum
					if( !string.IsNullOrWhiteSpace( temp = GetElementValue( x, ns, "payment_method" ) ) )
						resultOrder.PaymentMethod = temp.ToDecimalDotOrComaSeparated();

					var addresses = x.Element( ns + "addresses" );
					if( addresses != null )
					{
						var addressDataItems = addresses.Descendants( ns + "data_item" ).ToList();
						resultOrder.Addresses = addressDataItems.Select( addr =>
						{
							var address = new Address();
							address.Region = GetElementValue( addr, ns, "region" );
							address.Postcode = GetElementValue( addr, ns, "postcode" );
							address.Lastname = GetElementValue( addr, ns, "lastname" );
							address.Street = GetElementValue( addr, ns, "Street" );
							address.City = GetElementValue( addr, ns, "city" );
							address.Email = GetElementValue( addr, ns, "email" );
							address.Telephone = GetElementValue( addr, ns, "telephone" );
							address.CountryId = GetElementValue( addr, ns, "country_id" );
							address.Firstname = GetElementValue( addr, ns, "firstname" );
							//todo: to enum
							address.AddressType = GetElementValue( addr, ns, "address_type" );
							address.Prefix = GetElementValue( addr, ns, "prefix" );
							address.Middlename = GetElementValue( addr, ns, "Middlename" );
							address.Suffix = GetElementValue( addr, ns, "suffix" );
							address.Company = GetElementValue( addr, ns, "company" );
							return address;
						} );
					}

					var orderItems = x.Element( ns + "order_items" );
					if( orderItems != null )
					{
						var orderItemsDataItems = orderItems.Descendants( ns + "data_item" );
						resultOrder.Items = orderItemsDataItems.Select( addr =>
						{
							var order = new Item();
							order.ItemId = GetElementValue( addr, ns, "item_id" );
							order.ParentItemId = GetElementValue( addr, ns, "parent_item_id" );
							order.Sku = GetElementValue( addr, ns, "sku" );
							order.Name = GetElementValue( addr, ns, "name" );

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "qty_canceled" ) ) )
								order.QtyCanceled = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "qty_invoiced" ) ) )
								order.QtyInvoiced = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "qty_ordered" ) ) )
								order.QtyOrdered = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "qty_refunded" ) ) )
								order.QtyRefunded = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "qty_shipped" ) ) )
								order.QtyShipped = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "price" ) ) )
								order.Price = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "base_price" ) ) )
								order.BasePrice = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "original_price" ) ) )
								order.OriginalPrice = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "base_original_price" ) ) )
								order.BaseOriginalPrice = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "tax_percent" ) ) )
								order.TaxPercent = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "tax_amount" ) ) )
								order.TaxAmount = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "base_tax_amount" ) ) )
								order.BaseTaxAmount = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "discount_amount" ) ) )
								order.DscountAmount = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "base_discount_amount" ) ) )
								order.BaseDiscountAmount = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "row_total" ) ) )
								order.RowTotal = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "base_row_total" ) ) )
								order.BaseRowTotal = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "price_incl_tax" ) ) )
								order.PriceInclTax = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "base_price_incl_tax" ) ) )
								order.BasePriceInclTax = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "row_total_incl_tax" ) ) )
								order.RawTotalInclTax = temp.ToDecimalDotOrComaSeparated();

							if( !string.IsNullOrWhiteSpace( temp = GetElementValue( addr, ns, "base_row_total_incl_tax" ) ) )
								order.BaseRowTotalInclTax = temp.ToDecimalDotOrComaSeparated();

							return order;
						} );
					}

					var orderComments = x.Element( ns + "order_comments" );
					if( orderComments != null )
					{
						var orderCommentsDataItems = orderComments.Descendants( ns + "data_item" );
						resultOrder.Comments = orderCommentsDataItems.Select( addr =>
						{
							var comment = new Comment();
							comment.IsCustomerNotified = GetElementValue( addr, ns, "is_customer_notified" );
							comment.IsVisibleOnFront = GetElementValue( addr, ns, "is_visible_on_front" );
							comment.CommentText = GetElementValue( addr, ns, "comment" );

							//todo: enum
							comment.Status = GetElementValue( addr, ns, "status" );

							return comment;
						} );
					}

					if( keepStremPosition )
						stream.Position = streamStartPos;

					return resultOrder;
				} ).ToList();

				return new GetOrdersResponse { Orders = orders };
			}
			catch( Exception ex )
			{
				var buffer = new byte[ stream.Length ];
				stream.Read( buffer, 0, ( int )stream.Length );
				var utf8Encoding = new UTF8Encoding();
				var bufferStr = utf8Encoding.GetString( buffer );
				throw new Exception( "Can't parse: " + bufferStr, ex );
			}
		}
	}

	public class Comment
	{
		public string IsCustomerNotified { get; set; }
		public string IsVisibleOnFront { get; set; }
		public string CommentText { get; set; }
		public string Status { get; set; }
	}

	public class Item
	{
		public string ItemId { get; set; }
		public string ParentItemId { get; set; }
		public string Sku { get; set; }
		public string Name { get; set; }
		public decimal QtyCanceled { get; set; }
		public decimal QtyInvoiced { get; set; }
		public decimal QtyOrdered { get; set; }
		public decimal QtyRefunded { get; set; }
		public decimal QtyShipped { get; set; }
		public decimal Price { get; set; }
		public decimal BasePrice { get; set; }
		public decimal OriginalPrice { get; set; }
		public decimal BaseOriginalPrice { get; set; }
		public decimal TaxPercent { get; set; }
		public decimal TaxAmount { get; set; }
		public decimal BaseTaxAmount { get; set; }
		public decimal DscountAmount { get; set; }
		public decimal BaseDiscountAmount { get; set; }
		public decimal RowTotal { get; set; }
		public decimal BaseRowTotal { get; set; }
		public decimal PriceInclTax { get; set; }
		public decimal BasePriceInclTax { get; set; }
		public decimal RawTotalInclTax { get; set; }
		public decimal BaseRowTotalInclTax { get; set; }
	}

	public class Address
	{
		public string Region { get; set; }
		public string Postcode { get; set; }
		public string Lastname { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string Email { get; set; }
		public string Telephone { get; set; }
		public string CountryId { get; set; }
		public string Firstname { get; set; }
		public string AddressType { get; set; }
		public string Prefix { get; set; }
		public string Middlename { get; set; }
		public string Suffix { get; set; }
		public string Company { get; set; }
	}
}