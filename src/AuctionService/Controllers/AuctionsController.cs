using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
    {
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        return _mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
    {
        var auction = _mapper.Map<Auction>(auctionDto);
        // TODO: add current user as the seller
        auction.Seller = "test";
        _context.Auctions.Add(auction);
        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
        {
            return BadRequest("Could not save changes to the DB");
        }
        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, UpdateAuctionDto auctionDto)
    {
        var auction = await _context.Auctions
        .Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        // TODO: add validation to check if the current user is the seller

        auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = auctionDto.Year ?? auction.Item.Year;

        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
        {
            return BadRequest("Problem saving changes");
        }

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);

        if (auction == null)
        {
            return NotFound();
        }

        // TODO: add validation to check if the current user is the seller

        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if (!result)
        {
            return BadRequest("Problem deleting the auction");
        }

        return Ok();
    }
    
}