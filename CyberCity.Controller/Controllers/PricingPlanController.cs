using AutoMapper;
using CyberCity.Application.Interface;
using CyberCity.DTOs.PricingPlans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CyberCity.Controller.Controllers
{
    [Route("api/pricing-plans")]
    [ApiController]
    public class PricingPlanController : ControllerBase
    {
        private readonly IPricingPlanService _pricingPlanService;
        private readonly IMapper _mapper;

        public PricingPlanController(IPricingPlanService pricingPlanService, IMapper mapper)
        {
            _pricingPlanService = pricingPlanService;
            _mapper = mapper;
        }

        /// <summary>
        /// GET /api/pricing-plans - Danh sách gói giá
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PricingPlanDto>>> GetPricingPlans([FromQuery] bool descending = true)
        {
            try
            {
                var pricingPlans = await _pricingPlanService.GetAllPricingPlansAsync(descending);
                return Ok(pricingPlans);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/pricing-plans/{id} - Lấy thông tin gói giá theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PricingPlanDto>> GetPricingPlan(Guid id)
        {
            try
            {
                var pricingPlan = await _pricingPlanService.GetPricingPlanByIdAsync(id);
                return Ok(pricingPlan);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/pricing-plans - Tạo gói giá
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<PricingPlanDto>> CreatePricingPlan([FromBody] CreatePricingPlanDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var pricingPlan = await _pricingPlanService.CreatePricingPlanAsync(createDto);
                return CreatedAtAction(nameof(GetPricingPlan), new { id = pricingPlan.Uid }, pricingPlan);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// PUT /api/pricing-plans/{id} - Cập nhật gói giá
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<PricingPlanDto>> UpdatePricingPlan(Guid id, [FromBody] UpdatePricingPlanDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var pricingPlan = await _pricingPlanService.UpdatePricingPlanAsync(id, updateDto);
                return Ok(pricingPlan);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// DELETE /api/pricing-plans/{id} - Xóa gói giá
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeletePricingPlan(Guid id)
        {
            try
            {
                var success = await _pricingPlanService.DeletePricingPlanAsync(id);
                if (!success)
                    return NotFound(new { message = "Pricing plan not found" });

                return Ok(new { message = "Pricing plan deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// GET /api/pricing-plans/{id}/exists - Kiểm tra gói giá có tồn tại
        /// </summary>
        [HttpGet("{id}/exists")]
        public async Task<ActionResult<bool>> PricingPlanExists(Guid id)
        {
            try
            {
                var exists = await _pricingPlanService.PricingPlanExistsAsync(id);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}