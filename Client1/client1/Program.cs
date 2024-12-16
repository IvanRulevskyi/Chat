using System.Text.Json;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Text.Json.Serialization;
using System.Net.Http.Json;


ServiceCollection services = new ServiceCollection();
services.AddHttpClient();
ServiceProvider serviceProvider = services.BuildServiceProvider();
IHttpClientFactory httpClientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
HttpClient httpClient = httpClientFactory.CreateClient();




while (true)
{
    Console.WriteLine("Добро пожаловать в 'Телеграм' ");
    Console.WriteLine("Нажмите 1: для создания учетной записи");
    Console.WriteLine("Нажмите 2: для входа в учетную запись");
    Console.WriteLine("Нажмите 0: для выхода из программы");
    Console.WriteLine();
    Console.Write("Сделайте выбор: ");
    int.TryParse(Console.ReadLine(), out int choice);

    if (choice == 0)
    {
        break;
    }
    if (choice == 1)
    {
        Console.Write("Введите ИМЯ пользователя: ");
        string name = Console.ReadLine().ToLower();
        Console.Write("Введите ПАРОЛЬ: ");
        string password = Console.ReadLine().ToLower();
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Имя пользователя и пароль не могут быть пустыми.");
            continue;
        }
        var messageData = new 
        {
            LoginUser = name,
            PasswordUser = password
        };
        var jsonContent = new StringContent(JsonSerializer.Serialize(messageData), Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync("http://localhost:7282/usercreation", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Данные отправлены!");
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }
        catch (Exception e)
        {
            
            Console.WriteLine(e.Message);
        }
    }

    if (choice == 2)
    {
        Console.Write("Введите ИМЯ пользователя для входа: ");
        string name = Console.ReadLine().ToLower();
        Console.Write("Введите ПАРОЛЬ для входа: ");
        string password = Console.ReadLine().ToLower();

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Имя пользователя и пароль не могут быть пустыми.");
            continue;
        }
        var messageData = new 
        {
            LoginUser = name,
            PasswordUser = password
        };
        var jsonContent = new StringContent(JsonSerializer.Serialize(messageData), Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync("http://localhost:7282/user", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Данные отправлены!");
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            bool isSuccess = bool.Parse(responseBody);
            if (isSuccess == true)
            {
                Console.WriteLine($"Вход в профиль успешен {name}");
                Console.WriteLine();
                while (true)
                {
                    Console.WriteLine("Нажмите 1: для добавления контактов");
                    Console.WriteLine("Нажмите 2: для удаления контактов");
                    Console.WriteLine("Нажмите 3: для отправки сообщения контакту контактов");
                    Console.WriteLine("Нажмите 0: выхода из меню");
                    Console.WriteLine();
                    Console.Write("Сделайте выбор: ");
                    int.TryParse(Console.ReadLine(), out int newChoice);
                    if (newChoice == 0)
                    {
                        break;
                    }
                    if (newChoice == 1)
                    {
                        int myId = 0;
                        while (true)
                        {
                            myId = 0;
                            var jsonResponse = await httpClient.GetStringAsync("http://localhost:7282/searchuser");
                            var logins = JsonSerializer.Deserialize<List<User>>(jsonResponse);
                            if (logins != null)
                            {
                                
                                Console.WriteLine("Пользователи для добавления:");
                                foreach (var login in logins)
                                {
                                    if (login.loginUser == name)
                                    {
                                        myId += login.id;
                                        continue;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{login.id} {login.loginUser}");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Пользователей нет для добавления!!!");
                            }
                            
                            Console.Write("Введите имя контакта для добавления: ");
                            string nameAddContact = Console.ReadLine().ToLower();
                            foreach (var login in logins)
                            {
                                if (login.loginUser == nameAddContact)
                                {
                                    var messageDataAdd = new 
                                    {
                                        UserId = myId,
                                        ContactId = login.id
                                    };
                                    var jsonContentAdd = new StringContent(JsonSerializer.Serialize(messageDataAdd), Encoding.UTF8, "application/json");

                                    var addContact = await httpClient.PostAsync("http://localhost:7282/addcontact", jsonContentAdd);
                                    if (addContact.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine("Данные отправлены!");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Request failed with status code: {addContact.StatusCode}");
                                    }
                                    string contactAddResult = await addContact.Content.ReadAsStringAsync();
                                    Console.WriteLine(contactAddResult);
                                }   
                            }
                            Console.WriteLine("Нажмите 1: для продолжения добавления контактов");
                            Console.WriteLine("Нажмите 0: для выхода из меню");
                            Console.WriteLine();
                            Console.Write("Сделайте выбор: ");
                            int.TryParse(Console.ReadLine(), out int choiceMenu);
                            if (choiceMenu == 1)
                            {
                                continue;
                            }
                            if (choiceMenu == 0)
                            {
                                break;
                            }
                            
                        }

                    }

                    if (newChoice == 2)
                    {
                        int myId = 0;
                        while (true)
                        {
                            myId = 0;
                            var messageDataSearch = new
                            {
                                LoginUser = name
                            };
                            var contentSearch = new StringContent(JsonSerializer.Serialize(messageDataSearch), Encoding.UTF8, "application/json");
                            var jsonResponse = await httpClient.PostAsync("http://localhost:7282/searchcontact", contentSearch);
                            if (jsonResponse.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Данные отправлены!");
                            }
                            else
                            {
                                Console.WriteLine($"Request failed with status code: {jsonResponse.StatusCode}");
                            }
                            
                            string responseBodySearch = await jsonResponse.Content.ReadAsStringAsync();
                            var contacts = JsonSerializer.Deserialize<List<Contacts>>(responseBodySearch);
                            if (contacts != null)
                            {
                                Console.WriteLine("Пользователи для удаления:");
                                foreach (var login in contacts)
                                {
                                    Console.WriteLine($"Contact name: {login.contactName}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Пользователей нет для удаления!!!");
                                break;
                            }
                            Console.Write("Введите имя контакта для удаления: ");
                            string contactName = Console.ReadLine().ToLower();
                            var deleteContact = await httpClient.DeleteAsync($"http://localhost:7282/deletecontact/{contactName}/{name}");
                            if (deleteContact.IsSuccessStatusCode)
                            {
                                Console.WriteLine("Данные отправлены!");
                            }
                            else
                            {
                                Console.WriteLine($"Request failed with status code: {deleteContact.StatusCode}");
                            }
                            string contactAddResult = await deleteContact.Content.ReadAsStringAsync();
                            Console.WriteLine(contactAddResult);
                            Console.WriteLine("Нажмите 1: для продолжения удаления контактов");
                            Console.WriteLine("Нажмите 0: для выхода из меню");
                            Console.WriteLine();
                            Console.Write("Сделайте выбор: ");
                            int.TryParse(Console.ReadLine(), out int choiceMenu);
                            if (choiceMenu == 1)
                            {
                                continue;
                            }
                            if (choiceMenu == 0)
                            {
                                break;
                            }
                        }
                    }
                    if (newChoice == 3)
                    {
                        while (true)
                        {
                            var messageDataSearch = new
                            {
                                LoginUser = name
                            };
                            var contentSearch = new StringContent(JsonSerializer.Serialize(messageDataSearch), Encoding.UTF8, "application/json");
                            var jsonResponse = await httpClient.PostAsync("http://localhost:7282/searchcontact", contentSearch);
                            string responseBodySearch = await jsonResponse.Content.ReadAsStringAsync();
                            var contacts = JsonSerializer.Deserialize<List<Contacts>>(responseBodySearch);
                            foreach (var contact in contacts)
                            {
                                Console.WriteLine($"Имя контакта: {contact.contactName}");
                            }

                            Console.Write("Введите имя абонента: ");
                            string nameSendMessage = Console.ReadLine().ToLower();
                            bool contactFound = false;
                            foreach (var contact in contacts)
                            {
                                if (contact.contactName == nameSendMessage)
                                {
                                    contactFound = true;
                                    while (true)
                                    {
                                        var allMessage = await httpClient.GetAsync($"http://localhost:7282/searchmessages/{nameSendMessage}/{name}");
                                        var allMessages = await allMessage.Content.ReadFromJsonAsync<List<Message>>();

                                        foreach (var message in allMessages)
                                        {
                                            Console.WriteLine($"{message.SenderName} : {message.Text}");
                                        }
                                        Console.WriteLine();
                                        Console.WriteLine("Для выхода из часа введите 'END' ");
                                        Console.Write("Введите текст сообщения: ");
                                        string? textMessageAdd = Console.ReadLine();
                                        if (textMessageAdd == "END")
                                        {
                                            break;
                                        }
                                        var dataAddMessage = new
                                        {
                                            userName = name,
                                            contactName = nameSendMessage,
                                            textMessage = textMessageAdd
                                        };

                                        var contentAddMessage = new StringContent(JsonSerializer.Serialize(dataAddMessage), Encoding.UTF8, "application/json");
                                        var jsonResponseAddMes = await httpClient.PostAsync("http://localhost:7282/addmessage", contentAddMessage);
                                        if (jsonResponseAddMes.IsSuccessStatusCode)
                                        {
                                            Console.WriteLine("Сообщение отправлено!");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Request failed with status code: {jsonResponseAddMes.StatusCode}");
                                        }
                                    }
                                }
                            }
                            if (!contactFound)
                            {
                                Console.WriteLine("Вы ввели неправильное имя!");
                            }
                            Console.WriteLine("Нажмите 1: для продолжения отправки смс другому контакту");
                            Console.WriteLine("Нажмите 0: для выхода из меню");
                            Console.WriteLine();
                            Console.Write("Сделайте выбор: ");
                            int.TryParse(Console.ReadLine(), out int choiceMenu);
                            if (choiceMenu == 1)
                            {
                                continue;
                            }
                            if (choiceMenu == 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            if(isSuccess == false)
            {
                Console.WriteLine($"Пользователь {name} не найдет");
            }
        }
        catch (Exception e)
        {
            
            Console.WriteLine(e.Message);
        }

    }
}

public record User(int id, string loginUser);
public record Contacts(int contactId, string contactName);
public record AddMessage(string userName, string contactName, string textMessage);
public record Message(int Id, DateTime DateTime, string Text, int SenderId, int RecipientId, string SenderName, string RecipientName);

// Друге завдання
// Створіть мережевий додаток, який дозволить користувачам спілкуватися між
// собою через повідомлення.
// При першому використанні додатка користувач реєструється, а при наступ-
// них запусках користувач входить за створеним логіном і паролем. Користувач
// може надсилати запрошення для спілкування існуючому користувачеві, ство-
// рювати свій список контактів, додавати, видаляти, перейменовувати контакти.
// Користувачі можуть обмінюватися текстовими повідомленнями, картинками,
// файлами. Користувач може організувати груповий чат. Додаток має бути зі
// зручним користувацьким інтерфейсом.