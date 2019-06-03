using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Certitrack.Data;
using Certitrack.Models;
using Certitrack.Extensions.Alerts;

namespace certitrack_certificate_manager.Controllers
{
    public class ChannelsController : Controller
    {
        private readonly CertitrackContext _context;

        public ChannelsController(CertitrackContext context)
        {
            _context = context;
        }

        // GET: Channels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Channel.ToListAsync());
        }

        // GET: Channels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channel = await _context.Channel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (channel == null)
            {
                return NotFound();
            }

            return View(channel);
        }

        // GET: Channels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Channels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChannelName")] Channel channel)
        {
            if (ModelState.IsValid)
            {
                if (ChannelExists(channel.ChannelName))
                    return View(channel)
                        .WithWarning("Channel Exists", "Channel already exists. Try a different name.");
                _context.Add(channel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index))
                    .WithSuccess("Success", channel.ChannelName + " channel created successfully");
            }
            return View(channel);
        }

        // GET: Channels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channel = await _context.Channel.FindAsync(id);
            if (channel == null)
            {
                return NotFound();
            }
            return View(channel);
        }

        // POST: Channels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChannelName")] Channel channel)
        {
            if (id != channel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (ChannelExists(channel.ChannelName))
                    return View(channel)
                        .WithWarning("Channel Exists", "Channel already exists. Try a different name.");
                try
                {
                    _context.Update(channel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChannelExists(channel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index))
                    .WithSuccess("Update Successful", channel.ChannelName + " updated successfully"); ;
            }
            return View(channel)
                .WithWarning("Uh-Oh!", "Something went wrong, try again.");
        }

        // GET: Channels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var channel = await _context.Channel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (channel == null)
            {
                return NotFound();
            }

            return View(channel);
        }

        // POST: Channels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<string> DeleteConfirmed(int id)
        {
            try
            {
                var channel = await _context.Channel.FindAsync(id);
                _context.Channel.Remove(channel);
                await _context.SaveChangesAsync();
                return channel.ChannelName + " channel deleted successfully";
            }
            catch (Exception e)
            {
                if (!ChannelExists(id))
                {
                    return "Channel Id #" + id + " doesn't exist";
                }
                else
                {
                    return e.ToString();
                    throw;
                }
            }
        }

        private bool ChannelExists(int id)
        {
            return _context.Channel.Any(e => e.Id == id);
        }

        private bool ChannelExists(string name)
        {
            return _context.Channel.Any(e => e.ChannelName == name);
        }
    }
}
