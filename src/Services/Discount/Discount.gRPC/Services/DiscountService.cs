using Discount.gRPC.Data;
using Discount.gRPC.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.gRPC.Services;

public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        // TODO: GetDiscount from database
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(coupon => coupon.ProductName == request.ProductName);

        coupon ??= new Coupon { ProductName = "No discount", Amount = 0, Description = "No description" };

        CouponModel couponModel = coupon.Adapt<CouponModel>();

        logger.LogInformation("Discount is retrieved for ProductName: {ProductName}", coupon.ProductName);

        return couponModel;
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>() ?? throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid argument provided"));

        dbContext.Coupons.Add(coupon);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Discount is successfully created. ProductName: {ProductName}", coupon.ProductName);

        return request.Coupon;
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>() ?? throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid argument provided"));

        dbContext.Coupons.Update(coupon);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Discount is successfully updated. ProductName: {ProductName}", coupon.ProductName);

        return request.Coupon;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        string productName = request.ProductName ?? throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid argument provided"));

        var coupon = dbContext.Coupons.FirstOrDefault(coupon => coupon.ProductName == productName) ?? throw new RpcException(new Status(StatusCode.NotFound, "Discount not found"));

        dbContext.Coupons.Remove(coupon);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Discount is successfully deleted. ProductName: {ProductName}", productName);

        return new DeleteDiscountResponse { Success = true};
    }
}
