using Discount.gRPC.Data;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.gRPC.Services;

public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) : DicountProtoService.DicountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        // TODO: GetDiscount from database
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(coupon => coupon.ProductName == request.ProductName);

        coupon ??= new Models.Coupon { ProductName = "No discount", Amount = 0, Description = "No description" };

        CouponModel couponModel = coupon.Adapt<CouponModel>();

        return couponModel;
    }

    public override Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        return base.CreateDiscount(request, context);
    }

    public override Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        return base.UpdateDiscount(request, context);
    }

    public override Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        return base.DeleteDiscount(request, context);
    }
}
