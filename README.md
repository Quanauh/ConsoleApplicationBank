# Đăng ký API Endpoint trong ASP.NET Core Minimal API

## 1. API Endpoint là gì?

Trong ASP.NET Core Minimal API, **endpoint** là một địa chỉ API mà client có thể gửi HTTP Request tới để thực hiện một chức năng.

Ví dụ:

```text
GET /accounts
POST /login
PUT /accounts/123
DELETE /accounts/123
```

Trong `Program.cs`, endpoint được đăng ký bằng các phương thức như:

```csharp
app.MapGet(...)
app.MapPost(...)
app.MapPut(...)
app.MapPatch(...)
app.MapDelete(...)
```
Khởi tạo 1 project web api

```bash
dotnet new webapi -n HuanBank.Api
```
---

## 2. Cấu trúc cơ bản

Cú pháp chung:

```csharp
app.MapXXX("/duong-dan", () =>
{
    // Code xử lý request

    return ...;
});
```

Trong đó:

* `MapXXX` → loại HTTP Method.
* `"/duong-dan"` → URL của endpoint.
* `() => { ... }` → lambda expression, chính là hàm xử lý khi endpoint được gọi.

Ví dụ:

```csharp
app.MapGet("/hello", () =>
{
    return "Hello";
});
```

Khi client gửi:

```http
GET /hello
```

thì lambda sẽ được thực thi.

---

# 3. Các kiểu đăng ký API

## 3.1. MapGet()

Dùng cho HTTP `GET`.

Thường dùng để **lấy dữ liệu**.

```csharp
app.MapGet("/accounts", () =>
{
    return "Danh sách tài khoản";
});
```

Request:

```http
GET /accounts
```

Ví dụ lấy một tài khoản:

```csharp
app.MapGet("/accounts/{stk}", (string stk) =>
{
    return $"Thông tin tài khoản {stk}";
});
```

Request:

```http
GET /accounts/123456
```

`{stk}` là route parameter và giá trị `123456` sẽ được truyền vào biến `stk`.

---

## 3.2. MapPost()

Dùng cho HTTP `POST`.

Thường dùng khi **gửi dữ liệu lên server**, tạo dữ liệu hoặc thực hiện một hành động.

Ví dụ đăng nhập:

```csharp
app.MapPost("/login", (LoginRequest req) =>
{
    // Xử lý đăng nhập

    return "Đăng nhập thành công";
});
```

Client có thể gửi:

```json
{
    "stk": "123456",
    "mk": "123456"
}
```

Trong trường hợp này:

```csharp
(LoginRequest req)
```

là dữ liệu được nhận từ request body.

Ví dụ với project ngân hàng:

```csharp
app.MapPost("/transfer", (TransferRequest req) =>
{
    // Xử lý chuyển tiền

    return "Chuyển tiền thành công";
});
```

---

## 3.3. MapPut()

Dùng cho HTTP `PUT`.

Thường dùng để **cập nhật/thay thế toàn bộ thông tin của một resource**.

Ví dụ:

```csharp
app.MapPut("/accounts/{stk}", (string stk, Account account) =>
{
    // Cập nhật toàn bộ thông tin tài khoản

    return "Cập nhật thành công";
});
```

Request:

```http
PUT /accounts/123456
```

Body:

```json
{
    "ten": "Nguyen Van A",
    "diaChi": "Ha Noi"
}
```

---

## 3.4. MapPatch()

Dùng cho HTTP `PATCH`.

Thường dùng khi muốn **cập nhật một phần dữ liệu**.

Ví dụ tài khoản có:

```text
STK
Tên
Địa chỉ
Số điện thoại
```

Chỉ muốn sửa địa chỉ:

```csharp
app.MapPatch("/accounts/{stk}", (string stk, AccountUpdate data) =>
{
    // Chỉ cập nhật địa chỉ

    return "Cập nhật thành công";
});
```

Request:

```http
PATCH /accounts/123456
```

Body:

```json
{
    "diaChi": "Hai Phong"
}
```

---

## 3.5. MapDelete()

Dùng cho HTTP `DELETE`.

Thường dùng để **xóa dữ liệu**.

```csharp
app.MapDelete("/accounts/{stk}", (string stk) =>
{
    // Xóa tài khoản

    return "Xóa thành công";
});
```

Request:

```http
DELETE /accounts/123456
```

---

# 4. So sánh các Map

| Method        | HTTP   | Mục đích                             | Ví dụ         |
| ------------- | ------ | ------------------------------------ | ------------- |
| `MapGet()`    | GET    | Lấy dữ liệu                          | Lấy tài khoản |
| `MapPost()`   | POST   | Tạo/gửi dữ liệu, thực hiện hành động | Đăng nhập     |
| `MapPut()`    | PUT    | Cập nhật/thay thế                    | Sửa tài khoản |
| `MapPatch()`  | PATCH  | Cập nhật một phần                    | Sửa địa chỉ   |
| `MapDelete()` | DELETE | Xóa dữ liệu                          | Xóa tài khoản |

Có thể nhớ đơn giản:

```text
GET     → Lấy
POST    → Tạo / Gửi / Thực hiện
PUT     → Cập nhật toàn bộ
PATCH   → Cập nhật một phần
DELETE  → Xóa
```

---

# 5. Lambda trong MapGet/MapPost

Ví dụ:

```csharp
app.MapGet("/hello", () =>
{
    return "Hello";
});
```

Phần:

```csharp
() =>
{
    return "Hello";
}
```

là **lambda expression**.

Có thể hình dung nó tương đương với một hàm có tên:

```csharp
string XuLyHello()
{
    return "Hello";
}

app.MapGet("/hello", XuLyHello);
```

Nhưng lambda không cần đặt tên nên rất tiện khi hàm chỉ được sử dụng tại một endpoint.

---

# 6. Lambda có tham số

Lambda có thể nhận tham số:

```csharp
app.MapGet("/accounts/{stk}", (string stk) =>
{
    return $"Tài khoản: {stk}";
});
```

Ở đây:

```csharp
(string stk) => ...
```

có nghĩa là lambda nhận một tham số `stk`.

Khi request:

```http
GET /accounts/123456
```

thì:

```text
stk = "123456"
```

---

# 7. Ví dụ áp dụng vào Web API ngân hàng

Một `Program.cs` đơn giản có thể đăng ký:

```csharp
app.MapGet("/accounts", () =>
{
    // Lấy danh sách tài khoản
});

app.MapGet("/accounts/{stk}", (string stk) =>
{
    // Lấy một tài khoản
});

app.MapPost("/login", (LoginRequest req) =>
{
    // Đăng nhập
});

app.MapPost("/accounts", (Account account) =>
{
    // Tạo tài khoản
});

app.MapPost("/transfer", (TransferRequest req) =>
{
    // Chuyển tiền
});

app.MapPut("/accounts/{stk}", (string stk, Account account) =>
{
    // Cập nhật tài khoản
});

app.MapPatch("/accounts/{stk}", (string stk, AccountUpdate data) =>
{
    // Cập nhật một phần tài khoản
});

app.MapDelete("/accounts/{stk}", (string stk) =>
{
    // Xóa tài khoản
});
```

---

# 8. Tổng kết

Minimal API sử dụng các phương thức `Map...` để đăng ký endpoint:

```text
                 API Endpoint
                      │
          ┌───────────┼───────────┐
          │           │           │
        MapGet     MapPost      MapPut
          │           │           │
        Lấy         Gửi/Tạo     Cập nhật
                      
        MapPatch                  MapDelete
           │                          │
      Cập nhật một phần               Xóa
```

Công thức quan trọng nhất cần nhớ:

```csharp
app.MapXXX("URL", handler);
```

Trong đó `handler` thường là một lambda:

```csharp
() =>
{
    // Code xử lý request
}
```

Ví dụ:

```csharp
app.MapPost("/login", (LoginRequest req, BankManager qlnh) =>
{
    var kq = qlnh.Login(req.Stk, req.Mk);

    return kq;
});
```

Có thể đọc đoạn trên thành:

> **Khi client gửi POST `/login`, nhận `LoginRequest` và `BankManager`, sau đó chạy hàm xử lý đăng nhập và trả kết quả về client.**
![alt text](image.png)

Sửa file http để test api 
Ví dụ test đăng nhập 
```http
@HuanBank_HostAddress = http://localhost:5128

POST {{HuanBank_HostAddress}}/login
Content-Type: application/json

{
  "Stk": "doquanghuan1",
  "Mk": "2"
}
```

# Các kiểu `Results` trong ASP.NET Core Minimal API

Trong **ASP.NET Core Minimal API**, `Results` là class có sẵn trong namespace:

```csharp
Microsoft.AspNetCore.Http
```

`Results` cung cấp các phương thức để tạo **HTTP Response** trả về cho client.

Ví dụ:

```csharp
app.MapGet("/hello", () =>
{
    return Results.Ok("Hello World");
});
```

Client sẽ nhận:

```http
HTTP/1.1 200 OK

Hello World
```

---

## 1. `Results.Ok()`

Trả về HTTP status code:

```text
200 OK
```

Dùng khi request được xử lý thành công.

### Ví dụ

```csharp
app.MapGet("/account", () =>
{
    var account = new
    {
        stk = "doquanghuan",
        name = "Huan"
    };

    return Results.Ok(account);
});
```

Response:

```json
{
    "stk": "doquanghuan",
    "name": "Huan"
}
```

Status:

```text
200 OK
```

Có thể hiểu:

> "Server xử lý thành công và đây là dữ liệu trả về."

---

# 2. `Results.Created()`

Trả về:

```text
201 Created
```

Thường dùng khi **tạo mới một resource thành công**.

Ví dụ tạo tài khoản:

```csharp
app.MapPost("/account", (Account account) =>
{
    // Lưu account vào database

    return Results.Created(
        $"/account/{account.Stk}",
        account
    );
});
```

Response:

```text
201 Created
```

Kèm theo thông tin resource vừa tạo.

### Khi nào dùng?

Ví dụ:

```text
POST /account
```

→ Tạo tài khoản mới thành công.

```text
POST /students
```

→ Tạo sinh viên mới thành công.

```text
POST /products
```

→ Tạo sản phẩm mới thành công.

---

# 3. `Results.BadRequest()`

Trả về:

```text
400 Bad Request
```

Dùng khi request **không hợp lệ hoặc không được chấp nhận**.

Ví dụ:

```csharp
app.MapPost("/login", (LoginRequest request) =>
{
    if (string.IsNullOrEmpty(request.Stk))
    {
        return Results.BadRequest(new
        {
            message = "Thiếu số tài khoản"
        });
    }

    return Results.Ok();
});
```

Response:

```http
400 Bad Request
```

```json
{
    "message": "Thiếu số tài khoản"
}
```

### Trường hợp của project HuanBank

Bạn đang có:

```csharp
TransactionResult.IncorrectPassword =>
    Results.BadRequest(new
    {
        message = "Sai mat khau"
    }),
```

Nghĩa là:

```text
Mật khẩu sai
      ↓
IncorrectPassword
      ↓
Results.BadRequest()
      ↓
HTTP 400
```

**Lưu ý:** `Results.BadRequest()` không tự biết mật khẩu sai.

Chính code của bạn xác định:

```csharp
TransactionResult.IncorrectPassword
```

sau đó bạn chọn trả HTTP 400.

---

# 4. `Results.NotFound()`

Trả về:

```text
404 Not Found
```

Dùng khi **resource cần tìm không tồn tại**.

Ví dụ:

```csharp
app.MapGet("/account/{stk}", (string stk) =>
{
    var account = FindAccount(stk);

    if (account == null)
    {
        return Results.NotFound(new
        {
            message = "Không tìm thấy tài khoản"
        });
    }

    return Results.Ok(account);
});
```

Nếu tìm:

```text
GET /account/123456
```

nhưng tài khoản không tồn tại:

```text
404 Not Found
```

---

# 5. `Results.Unauthorized()`

Trả về:

```text
401 Unauthorized
```

Dùng khi client **chưa được xác thực hoặc thông tin xác thực không hợp lệ**.

Ví dụ:

```csharp
app.MapGet("/profile", () =>
{
    return Results.Unauthorized();
});
```

Response:

```text
401 Unauthorized
```

Thường gặp trong các API sử dụng:

* JWT
* Cookie Authentication
* Authentication middleware

Ví dụ:

```text
Client
   ↓
GET /profile
   ↓
Không có token
   ↓
401 Unauthorized
```

---

# 6. `Results.Forbid()`

Trả về:

```text
403 Forbidden
```

Khác với `401`.

### `401`

> Chưa xác thực.

### `403`

> Đã xác thực nhưng **không có quyền** thực hiện hành động.

Ví dụ:

```text
User thường
   ↓
GET /admin/users
   ↓
Đã đăng nhập ✅
Nhưng không phải Admin ❌
   ↓
403 Forbidden
```

Trong code:

```csharp
return Results.Forbid();
```

---

# 7. `Results.NoContent()`

Trả về:

```text
204 No Content
```

Nghĩa là:

> Request xử lý thành công nhưng không có dữ liệu cần trả về.

Ví dụ xóa tài khoản:

```csharp
app.MapDelete("/account/{stk}", (string stk) =>
{
    DeleteAccount(stk);

    return Results.NoContent();
});
```

Response:

```text
204 No Content
```

Không có response body.

---

# 8. `Results.Conflict()`

Trả về:

```text
409 Conflict
```

Dùng khi request **xung đột với trạng thái hiện tại của server**.

Ví dụ tạo tài khoản nhưng số tài khoản đã tồn tại:

```csharp
if (AccountExists(request.Stk))
{
    return Results.Conflict(new
    {
        message = "Tài khoản đã tồn tại"
    });
}
```

Response:

```text
409 Conflict
```

Ví dụ:

```text
POST /account

stk = "123456"

       ↓

Database đã có 123456

       ↓

409 Conflict
```

---

# 9. `Results.StatusCode()`

Cho phép tự chỉ định HTTP status code.

Ví dụ:

```csharp
return Results.StatusCode(500);
```

→

```text
500 Internal Server Error
```

Hoặc:

```csharp
return Results.StatusCode(418);
```

→ HTTP status code `418`.

Tuy nhiên, nếu ASP.NET Core đã có method phù hợp như:

```csharp
Results.NotFound()
Results.BadRequest()
Results.Unauthorized()
```

thì nên ưu tiên chúng thay vì tự viết:

```csharp
Results.StatusCode(404)
```

---

# 10. `Results.Json()`

Dùng để trả về JSON.

Ví dụ:

```csharp
return Results.Json(new
{
    message = "Đăng nhập thành công",
    stk = "123456"
});
```

Response:

```json
{
    "message": "Đăng nhập thành công",
    "stk": "123456"
}
```

Thông thường bạn có thể dùng:

```csharp
Results.Ok(data)
```

vì ASP.NET Core cũng có thể serialize object thành JSON.

---

# 11. `Results.Text()`

Trả về text.

Ví dụ:

```csharp
return Results.Text("Hello World");
```

Client nhận:

```text
Hello World
```

Có thể chỉ định Content-Type:

```csharp
return Results.Text(
    "Hello World",
    "text/plain"
);
```

---

# 12. `Results.File()`

Dùng để trả file về cho client.

Ví dụ:

```csharp
return Results.File(
    fileBytes,
    "application/pdf",
    "report.pdf"
);
```

Client có thể nhận file:

```text
report.pdf
```

Thường dùng cho:

* PDF
* Excel
* hình ảnh
* file download

---

# 13. `Results.Redirect()`

Dùng để yêu cầu client chuyển hướng sang URL khác.

Ví dụ:

```csharp
return Results.Redirect("/login");
```

Client sẽ được yêu cầu chuyển sang:

```text
/login
```

HTTP thường trả:

```text
3xx Redirect
```

---

# 14. Bảng tổng hợp

| Method                   |  HTTP Status | Ý nghĩa                           |
| ------------------------ | -----------: | --------------------------------- |
| `Results.Ok()`           |          200 | Thành công                        |
| `Results.Created()`      |          201 | Tạo resource thành công           |
| `Results.NoContent()`    |          204 | Thành công, không có body         |
| `Results.BadRequest()`   |          400 | Request không hợp lệ / bị từ chối |
| `Results.Unauthorized()` |          401 | Chưa xác thực                     |
| `Results.Forbid()`       |          403 | Không có quyền                    |
| `Results.NotFound()`     |          404 | Không tìm thấy resource           |
| `Results.Conflict()`     |          409 | Xung đột                          |
| `Results.StatusCode()`   |     Tùy chọn | Tự chỉ định status                |
| `Results.Json()`         | 200 mặc định | Trả JSON                          |
| `Results.Text()`         | 200 mặc định | Trả text                          |
| `Results.File()`         | 200 mặc định | Trả file                          |
| `Results.Redirect()`     |          3xx | Chuyển hướng                      |

---

# 15. Cách chọn `Result` khi viết API

Có thể nhớ theo flow này:

```text
                    Request
                       │
                       ▼
              Request hợp lệ?
                /           \
              Không          Có
               │              │
               ▼              ▼
        BadRequest(400)   Xử lý tiếp
                              │
                              ▼
                       Resource tồn tại?
                         /          \
                       Không         Có
                        │             │
                        ▼             ▼
                 NotFound(404)    Xử lý tiếp
                                      │
                                      ▼
                               Có quyền không?
                                /          \
                              Không         Có
                               │             │
                               ▼             ▼
                         Forbid(403)     Thành công
                                             │
                                  ┌──────────┼──────────┐
                                  ▼          ▼          ▼
                                Ok(200) Created(201) NoContent(204)
```

---

# 16. Ví dụ API đăng nhập hoàn chỉnh

Ví dụ:

```csharp
app.MapPost("/login", (LoginRequest request) =>
{
    var result = Login(request);

    return result switch
    {
        TransactionResult.Success =>
            Results.Ok(new
            {
                message = "Đăng nhập thành công"
            }),

        TransactionResult.AccountNotFound =>
            Results.NotFound(new
            {
                message = "Không tìm thấy tài khoản"
            }),

        TransactionResult.IncorrectPassword =>
            Results.BadRequest(new
            {
                message = "Sai mat khau"
            }),

        _ =>
            Results.StatusCode(500)
    };
});
```

Ta có:

```text
Login thành công
       ↓
200 OK
```

```text
Không tìm thấy tài khoản
       ↓
404 Not Found
```

```text
Sai mật khẩu
       ↓
400 Bad Request
```

```text
Lỗi không xác định
       ↓
500 Internal Server Error
```

---

# 17. Điều quan trọng cần nhớ

`Results` **không phải là kết quả xử lý nghiệp vụ**.

Nó là cách để bạn **tạo HTTP Response**.

Ví dụ:

```csharp
TransactionResult.IncorrectPassword
```

là **kết quả nghiệp vụ**.

Còn:

```csharp
Results.BadRequest(...)
```

là **cách biến kết quả đó thành HTTP Response**.

Có thể hình dung:

```text
             BUSINESS LOGIC
                   │
                   ▼
       TransactionResult
                   │
          ┌────────┼─────────┐
          ▼        ▼         ▼
       Success   NotFound   IncorrectPassword
          │        │         │
          ▼        ▼         ▼
      Results.Ok  Results.  Results.BadRequest
                  NotFound
          │        │         │
          ▼        ▼         ▼
         200      404       400
```

Đây là một cách thiết kế khá tốt vì **logic nghiệp vụ và HTTP response được tách ra**.

> `TransactionResult` trả lời câu hỏi: **"Chuyện gì xảy ra?"**

> `Results` trả lời câu hỏi: **"Tôi muốn HTTP trả gì cho client?"**
