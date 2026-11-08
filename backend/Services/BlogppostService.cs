using Microsoft.EntityFrameworkCore;
using Whoami.Api.Data;
using Whoami.Api.Models;
using Whoami.Api.DTOs;

namespace Whoami.Api.Services;

public class BlogpostService
{
    private readonly AppDbContext _context;

    public BlogpostService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Blogpost>> GetAllAsync()
    {
        return await _context.Blogposts.ToListAsync();
    }

    public async Task<Blogpost> CreateAsync(BlogpostCreateDto dto)
    {
        var blogpost = new Blogpost
        {
            Titel = dto.Titel,
            Inhoud = dto.Inhoud,
            Publicatiedatum = dto.Publicatiedatum
        };

        _context.Blogposts.Add(blogpost);
        await _context.SaveChangesAsync();

        return blogpost;
    }

    public async Task<Blogpost?> GetByIdAsync(int id)
    {
        return await _context.Blogposts.FindAsync(id);
    }

    public async Task<bool> UpdateAsync(int id, BlogpostUpdateDto dto)
    {
        var blogpost = await _context.Blogposts.FindAsync(id);

        if (blogpost == null)
        {
            return false;
        }

        blogpost.Titel = dto.Titel;
        blogpost.Inhoud = dto.Inhoud;
        blogpost.Publicatiedatum = dto.Publicatiedatum;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var blogpost = await _context.Blogposts.FindAsync(id);

        if (blogpost == null)
        {
            return false;
        }

        _context.Blogposts.Remove(blogpost);
        await _context.SaveChangesAsync();

        return true;
    }
}