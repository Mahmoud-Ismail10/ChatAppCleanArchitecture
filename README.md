# 💬 Real-Time Chat Application

Real-Time Chat backend API built with **.NET 9**, **SignalR**, and **Clean Architecture**, designed for **instant communication**, **scalability**, and **maintainability**.

---

## 🎯 Project Goal
Build a real-time messaging platform supporting **private and group chats**, **message statuses**, and **live notifications**, with a clean, testable, and scalable architecture.

---

## 🏗 Architecture & Layers

- **Presentation Layer** – Exposes RESTful APIs and SignalR hubs  
- **Service Layer** – Contains core business logic and real-time operations  
- **Core Layer** – Implements CQRS (Commands, Queries), Mapping, and Filters  
- **Domain Layer** – Defines Entities, Enums, and Business Rules  
- **Infrastructure Layer** – Handles Database Context, Repositories, Configuration, and External Integrations  

This structure ensures **separation of concerns**, **testability**, and **clean maintainability**.

---

## 🛠 Tech Stack

- **.NET 9 Web API** – Clean Architecture  
- **Entity Framework Core + SQL Server**  
- **SignalR** – Real-time communication  
- **CQRS + MediatR** – Separation of commands and queries  
- **FluentValidation** – Strong request validation  
- **JWT Authentication** – Secure token-based access  
- **AutoMapper** – DTO ↔ Entity mapping  
- **Serilog** – Structured logging  
- **Localization** – Multi-language response support  
- **Redis (optional)** – Active users and connection caching  

---

## 🧱 Core Entities

- **User** – Represents a registered platform user  
- **Chat** – Either a private or group conversation  
- **ChatMember** – User participation in chats (with joined/left status)  
- **Message** – Text, image, voice, or file messages  
- **MessageStatus** – Tracks delivery state (Sent, Delivered, Seen)

---

## ⚡ Key Features

- 💬 **Private & Group Chats** – Real-time messaging between users and groups  
- 👀 **Message Status Tracking** – Sent, Delivered, and Seen indicators  
- 🧑‍🤝‍🧑 **Group Management** – Create, update, and delete groups; manage members  
- ⚡ **Live Updates via SignalR** – Instant UI sync for messages, chats, and members  
- 🟢 **Online Presence** – Track active and connected users  
- 🌐 **Localization Support** – Multi-language response messages  
- 🔔 **Notifications** – Real-time notifications for new messages or group events  

---

## 📡 API Endpoints Overview

### 🔐 Authentication
| Method | Endpoint | Description |
|:-------|:----------|:-------------|
| POST | `/api/v1/authenticate/sendOtp` | Send OTP for verification |
| POST | `/api/v1/authenticate/verifyOtp` | Verify OTP code |
| POST | `/api/v1/authenticate/register` | Register new user |
| POST | `/api/v1/authenticate/createSession` | Create user session |
| POST | `/api/v1/authenticate/logout` | Logout user session |

### 💬 Chats
| Method | Endpoint | Description |
|:-------|:----------|:-------------|
| GET | `/api/v1/chats/getChatWithMessages` | Retrieve chat messages |
| POST | `/api/v1/chats/createGroup` | Create new group chat |
| PUT | `/api/v1/chats/updateGroup` | Update group name or info |
| PUT | `/api/v1/chats/updateGroupImage` | Update group image |

### 👥 Chat Members
| Method | Endpoint | Description |
|:-------|:----------|:-------------|
| POST | `/api/v1/chatmembers/addMembersToGroup` | Add users to a group |
| PUT | `/api/v1/chatmembers/removeMemberFromGroup` | Remove a member from a group |
| PUT | `/api/v1/chatmembers/leftGroup` | User leaves a group |
| DELETE | `/api/v1/chatmembers/deleteGroup` | Delete a group and its members |

---

## 🔮 Future Enhancements

- 📁 **File Upload & Storage (Azure/AWS)**  
- 📱 **Push Notifications (FCM / OneSignal)**  
- ❤️ **Message Reactions & Replies**  
- 🗜️ **Media Compression & Preview Thumbnails**  
- 🔒 **End-to-End Encryption for Messages**  

---

## 💡 Why This Project Matters

- Demonstrates **SignalR real-time communication**  
- Implements **Clean Architecture + CQRS + MediatR**  
- Provides **modern chat system features** with message tracking  
- Designed for **extensibility**, **security**, and **high performance**

---

## 🏷 Tags

`.NET 9` `.AspNetCore` `.WebAPI` `.SignalR` `.CleanArchitecture` `.CQRS` `.MediatR` `.EFCore` `.FluentValidation` `.JWT` `.Serilog` `.Localization` `.Redis` `.ChatApplication` `.BackendDevelopment` `.MessagingSystem` `.RealTime` `.SoftwareEngineering`

