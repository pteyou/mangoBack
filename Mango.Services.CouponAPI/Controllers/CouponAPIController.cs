using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public CouponAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            var response = new ResponseDto();
            try
            {
                response.Result = _mapper.Map<IEnumerable<CouponDto>>(_db.Coupons.ToList());
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            var response = new ResponseDto();
            try
            {
                response.Result = _mapper.Map<CouponDto>(_db.Coupons.First(c => c.CouponId == id));
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto Get(string code)
        {
            var response = new ResponseDto();
            try
            {
                response.Result = _mapper.Map<CouponDto>(_db.Coupons.First(c => c.CouponCode.ToLower() == code.ToLower()));
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            var response = new ResponseDto();
            try
            {
                _db.Coupons.Add(_mapper.Map<Coupon>(couponDto));
                _db.SaveChanges();
                response.Result = couponDto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }


        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            var response = new ResponseDto();
            try
            {
                _db.Coupons.Update(_mapper.Map<Coupon>(couponDto));
                _db.SaveChanges();
                response.Result = couponDto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        public ResponseDto Delete(int id)
        {
            var response = new ResponseDto();
            try
            {
                var obj = _db.Coupons.First(c => c.CouponId == id);
                _db.Remove(obj);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
