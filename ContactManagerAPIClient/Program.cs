
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    static async Task Main(string[] args)
    {

        client.BaseAddress = new Uri("https://localhost:7140/api/ContactManagerAPI/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        //Base URL of your API
        string baseUrl = "https://localhost:7140/api/ContactManagerAPI/";

        //READ: Don't run all of these at once (even though technically you could). Run them individually to get cleaner focus on the task you want to test.
        //await CreateContactAsync();
        //await GetContactsAsync();
        //await SearchContactsAsync("Kevin Durant", Convert.ToDateTime("01-01-1988"), Convert.ToDateTime("04-01-1989"));
        //await UpdateContactAsync(3);
        await DeleteContactAsync(6);

    }

    #region GetContactsAsync
    static async Task GetContactsAsync()
    {
        var response = await client.GetAsync("GetContacts");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var contacts = JsonSerializer.Deserialize<List<ContactDTO>>(responseBody);

        foreach (var contact in contacts)
        {
            Console.WriteLine($"Contact ID: {contact.id}, Name: {contact.name}, Birth Date: {contact.birthDate}");
            foreach (var email in contact.emails)
            {
                Console.WriteLine($"  Email: {email.address}, IsPrimary: {email.isPrimary}");
            }
        }
    }
    #endregion

    #region CreateContactAsync
    static async Task CreateContactAsync()
    {
        var contact = new ContactDTO
        {
            name = "Amen Thompson",
            birthDate = DateOnly.Parse("2000-01-14"),
            emails = new List<EmailDTO>
                {
                    new EmailDTO { address = "athompson@example.com", isPrimary = true }
                }
        };

        var json = JsonSerializer.Serialize(contact);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("CreateContact", content);
        response.EnsureSuccessStatusCode();

        Console.WriteLine("Contact created successfully!");
    }
    #endregion

    #region SearchContactsAsync
    static async Task SearchContactsAsync(string name, DateTime? startDate, DateTime? endDate)
    {
        var response = await client.GetAsync($"SearchContact?name={name}&startDate={startDate}&endDate={endDate}");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var contacts = JsonSerializer.Deserialize<List<ContactDTO>>(responseBody);

        foreach (var contact in contacts)
        {
            Console.WriteLine($"Contact ID: {contact.id}, Name: {contact.name}, Birth Date: {contact.birthDate}");
        }
    }
    #endregion

    #region UpdateContactAsync
    static async Task UpdateContactAsync(int id)
    {
        var response = await client.GetAsync($"GetContact?id={id}");
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var contact = JsonSerializer.Deserialize<ContactDTO>(responseBody);

        contact.name = "Mikal Bridges";
        contact.emails.ForEach(e => e.address = "mbridges@example.com");

        var json = JsonSerializer.Serialize(contact);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        response = await client.PutAsync($"UpdateContact?id={id}", content);
        response.EnsureSuccessStatusCode();

        Console.WriteLine("Contact updated successfully!");
    }
    #endregion

    #region DeleteContactAsync
    static async Task DeleteContactAsync(int id)
    {
        var response = await client.DeleteAsync($"DeleteContact?id={id}");
        response.EnsureSuccessStatusCode();

        Console.WriteLine("Contact deleted successfully!");
    }
    #endregion
}

#region DTO Definitions
// Define ContactDTO class to match the structure of your API response
public class ContactDTO
{
    public int id { get; set; }
    public string name { get; set; }
    public DateOnly? birthDate { get; set; }
    public List<EmailDTO> emails { get; set; }
}

public class EmailDTO
{
    public int id { get; set; }
    public string address { get; set; }
    public bool isPrimary { get; set; }
}
#endregion