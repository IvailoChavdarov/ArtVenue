using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArtVenue.Data;
using ArtVenue.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ArtVenue.ViewModels;
using System.Globalization;

namespace ArtVenue.Controllers
{
    //responsible for creation and editing of user groups
    [Authorize]
    public class GroupsController : Controller
    {
        //dependency injection
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        public GroupsController(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _db = db;
        }

        //returns data for all the groups the current user has joined and sorts it by whether the user is creator of the group (to know if to display manage group buttons)
        public async Task<IActionResult> Index()
        {
            //gets current user data
            var user = await _userManager.GetUserAsync(User);

            List<Group> groupsCreated = new List<Group>();
            List<Group> groupsJoined = new List<Group>();

            //gets groups user has created if there are any
            if (_db.Groups.Where(x => x.CreatorId == user.Id).Any())
            {
                groupsCreated = await _db.Groups
                    .Where(x => x.CreatorId == user.Id)
                    .ToListAsync();
            }

            //gets ids of groups user has created
            int[] groupsCreatedIds = groupsCreated.Select(x => x.Id).ToArray();

            //gets groups user has joined
            foreach (var membership in _db.Groups_Members.Where(x=>x.MemberId == user.Id))
            {
                //checks if group is created by user
                if (!groupsCreatedIds.Contains(membership.GroupId))
                {
                    Group groupJoined = await _db.Groups.FindAsync(membership.GroupId);
                    groupsJoined.Add(groupJoined);
                }
            }

            //sets data and sends it to view
            GroupsIndexViewModel data = new GroupsIndexViewModel()
            {
                GroupsCreated = groupsCreated,
                GroupsJoined = groupsJoined
            };

            return View(data);
        }

        //returns view with form for creating new group
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //gets the data from the form and adds the new group to the database
        public async Task<IActionResult> Create(Group group)
        {
            //sets the new group's creator to current user
            var user = await _userManager.GetUserAsync(User);
            group.CreatorId = user.Id;


            try
            {
                //adds group to database
                _db.Groups.Add(group);

                //adds group creator to group members
                _db.Groups_Members.Add(new Groups_Members()
                {
                    MemberId = user.Id,
                    Group = group,
                    JoinedDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm tt", CultureInfo.InvariantCulture)
                }) ;

                //saves changes to database
                await _db.SaveChangesAsync();

                //sends success notice to view
                TempData["notice"] = $"Successfully created group!";
                TempData["noticeBackground"] = "bg-success";

                //redirects to main groups page
                return RedirectToAction("index");
            }
            catch
            {
                //an error happened while adding group to database
                //sends fail notice
                TempData["notice"] = $"Couldn't create group!";
                TempData["noticeBackground"] = "bg-danger";

                //redirects to group creation page with entered data
                return View(group);
            }

        }
    
        //returns view with data and update form for specific group the user has created by id (edit group page)
        public async Task<IActionResult> Edit(int id)
        {
            var group = await _db.Groups.FindAsync(id);
            //checks if group exists
            if (group == null)
            {
                return NotFound();
            }

            //checks if current user is creator of group
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }

            //returns view if user has access to edit group
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //gets the new data for the groups and updates the data in the database
        public async Task<IActionResult> UpdateGroup(Group group)
        {
            //validates if user has access to edit group
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }
            try
            {
                //uptdates database
                _db.Update(group);
                await _db.SaveChangesAsync();

                //sends success notice and returns to main groups page
                TempData["notice"] = $"Successfully updated group!";
                TempData["noticeBackground"] = "bg-success";

                return RedirectToAction("index");
            }
            catch
            {
                //an error happened while updating group
                //sends fail notice
                TempData["notice"] = $"Couldn't update group!";
                TempData["noticeBackground"] = "bg-danger";

                //redirects to group edit page with entered data
                return View(group);
            }
        }

        //returns view with data for specific group the user has created and wants to delete (confirm group deletion page)
        public async Task<IActionResult> Delete(int id)
        {
            //gets group
            var group = await _db.Groups.FindAsync(id);
            //validates that group exists
            if (group==null)
            {
                return NotFound();
            }

            //validates that current user is creator of group
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }

            //returns confirm delete page
            return View(group);
        }

        //deletes specific group the user has created by id of group and returns to the page listing user's groups
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _db.Groups.FindAsync(id);

            //validates that group exists
            if (group == null)
            {
                return NotFound();
            }

            //validates that current user is creator of group
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }

            try
            {
                //removes group from database
                _db.Groups.Remove(group);
                await _db.SaveChangesAsync();

                //sends success notice and returns to main groups page
                TempData["notice"] = $"Successfully deleted group!";
                TempData["noticeBackground"] = "bg-success";

                return RedirectToAction("index");
            }
            catch
            {
                //an error happened while updating group
                //sends fail notice
                TempData["notice"] = $"Couldn't delete group!";
                TempData["noticeBackground"] = "bg-danger";

                //redirects to group edit page with entered data
                return View(group);
            }
        }

        //returns view with data for all members of specific group the user has created by group id
        public async Task<IActionResult> Members(int id)
        {
            var group = await _db.Groups.FindAsync(id);

            //validates that group exists
            if (group == null)
            {
                return NotFound();
            }

            //validates that current user is creator of group
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }

            List<Groups_Members> memberships = new List<Groups_Members>();


            //gets group members
            List<Groups_Members> group_members = await _db.Groups_Members.Where(x => x.GroupId == id).ToListAsync();
            foreach (var membership in group_members)
            {
                if (membership.MemberId != group.CreatorId)
                {
                    membership.Member = await _userManager.FindByIdAsync(membership.MemberId);
                    memberships.Add(membership);
                }
            }

            //sets members data to model and sends data to view
            GroupsMembersViewModel data = new GroupsMembersViewModel() {
                GroupName = group.GroupName,
                Memberships = memberships
            };

            return View(data);
        }

        //removes member from group by member id and group id and returns to the page listing the changed group's members
        [HttpPost]
        public async Task<IActionResult> RemoveMember(int groupId, string userId)
        {
            var group = await _db.Groups.FindAsync(groupId);

            //validates that group exists
            if (group == null)
            {
                return NotFound();
            }

            //validates that current user is creator of group
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }

            var membership = await _db.Groups_Members
                .Where(x => x.GroupId == groupId && x.MemberId == userId)
                .FirstOrDefaultAsync();

            //validates that given user is member of group
            if (membership == null)
            {
                return NotFound();
            }

            //removes membership from database
            _db.Groups_Members.Remove(membership);
            await _db.SaveChangesAsync();

            return RedirectToAction("members", new { Id = groupId });
        }

        //returns view with data for the requests to join group the user has created by group id
        public async Task<IActionResult> Requests(int id)
        {
            var group = await _db.Groups.FindAsync(id);

            //validates that group exists
            if (group == null)
            {
                return NotFound();
            }

            //validates that current user is creator of group
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }

            List<Groups_Requests> requests = new List<Groups_Requests>();

            //checks if there are any join requests for group
            if (await _db.Groups_Requests.Where(x => x.GroupId == id).AnyAsync())
            {
                //gets request related data
                foreach (var request in _db.Groups_Requests.Where(x => x.GroupId == id))
                {
                    //connects request to user
                    request.Member = await _userManager.FindByIdAsync(request.MemberId);
                    requests.Add(request);
                }
            }

            //sets requests data to model and sends data to view
            GroupsRequestsViewModel data = new GroupsRequestsViewModel()
            {
                GroupName = group.GroupName,
                Requests = requests
            };
            return View(data);
        }

        //removes specific join request for group the user has created by group id and the user that requested to join id
        [HttpPost]
        public async Task<IActionResult> DeleteRequest(int groupId, string userId)
        {
            var group = await _db.Groups.FindAsync(groupId);

            //validates that group exists
            if (group == null)
            {
                return NotFound();
            }

            //validates that current user is creator of group
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }

            var request = await _db.Groups_Requests
                .Where(x => x.GroupId == groupId && x.MemberId == userId)
                .FirstOrDefaultAsync();

            //validates that request exists
            if (request==null)
            {
                return NotFound();
            }

            //removes request from database
            _db.Groups_Requests.Remove(request);
            await _db.SaveChangesAsync();

            //returns to group join request page
            return RedirectToAction("requests", new {Id=groupId });
        }

        //adds user that requested to join group to the specific group by group's id and user that requested to join id
        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int groupId, string userId)
        {
            var group = await _db.Groups.FindAsync(groupId);

            //validates that group exists
            if (group == null)
            {
                return NotFound();
            }

            //validates that current user is creator of group
            if (!IsCreatorOf(group))
            {
                return Forbid();
            }

            var request = await _db.Groups_Requests
                .Where(x => x.GroupId == groupId && x.MemberId == userId)
                .FirstOrDefaultAsync();

            //validates that request exists
            if (request == null)
            {
                return NotFound();
            }

            //clears request
            _db.Groups_Requests.Remove(request);

            //adds user to group
            _db.Groups_Members.Add(new Groups_Members()
            {
                GroupId = groupId,
                MemberId = userId,
                JoinedDate = DateTime.Now.ToString("dd MMMM yyyy HH:mm tt", CultureInfo.InvariantCulture)
            }) ;

            await _db.SaveChangesAsync();

            //returns to group join request page
            return RedirectToAction("requests", new { Id = groupId });
        }

        //checks if current user is creator or given group
        private bool IsCreatorOf(Group group)
        {
            return group.CreatorId == _userManager.GetUserId(User);
        }
    }
}
