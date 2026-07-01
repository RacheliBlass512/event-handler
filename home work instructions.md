# Home Assignment – Experienced Developer / Team Leader

**Working Time:** One calendar week 
**Technologies:** .NET Core / Python + Angular + SQL Server 

### General Clarifications
* Using AI tools, official documentation, search engines, and existing packages is allowed and even encouraged.
* The purpose of the test is not to check API memorization, but rather planning capability, solution selection, output control, technological reasoning, and delivering a high-quality working flow.
* The use of AI tools does not absolve the candidate from full responsibility for the deliverables.
* The candidate must deeply understand every architectural decision, code block, and technological choice submitted as if they had written it from scratch.
* It is mandatory to explicitly justify in the architecture document every major decision and choice: why a specific solution was chosen, what alternatives were considered and rejected, and what trade-offs led to the final decision.
* A document without reasoning will be weighed accordingly.
* During the Code Review, the candidate will be asked to explain and justify every part of the solution; answers such as "the AI suggested it" are unacceptable.
* The candidate must briefly state in the architecture document or README which AI tools or help sources were used, and what was tested, changed, or rejected from the suggestions received.
* This is not a formal transparency requirement, but part of the test itself: did the candidate understand what the AI suggested, exercise critical judgment, and adapt the solution to the specific requirements of the system?
* The emphasis in the evaluation is on engineering judgment, code organization, understanding trade-offs, and the ability to lead a focused solution within a complex system.

### 1. Background and General Description
* The organization operates field teams required to handle events arriving from various sources: sensors, external systems, and manual reports.
* Currently, coordination is done by phone, causing delays, loss of information, and a lack of transparency.
* The goal is to build a centralized real-time field event management system – from the moment the event enters the system until it is closed, with continuous communication between the dispatcher and the field technician.
* Each event contains a small amount of information (title, short description, source, location, etc.).
* The system is not intended for processing large volumes of data, files, or images, but for transmitting short messages in real-time.

### 2. System Components
The system consists of three central components:

#### A. Central Agent – Listens for Events
* An independent service component running continuously in the background.
* Its role is to collect events from multiple sources and forward them to the central server.
* The candidate must plan and justify:
    * The Agent architecture (remains intentionally open).
    * Communication mechanism between the Agent and the central server .
    * How the Agent is exposed to the external world.
    * At least two technological alternatives considered for the Agent implementation and why the final one was chosen.
    * How each source reporting an event is authenticated.
    * What happens when the central server is unavailable (how the event is not lost).
    * How a new source can be easily connected to the system.

#### B. Central Server (Backend)
* Receives events from the Agent, manages business logic, and communicates with clients.
* Responsibilities:
    * Receiving events from the Agent in real-time.
    * Managing a State Machine for each event.
    * Managing permissions and separation between user types.
    * Sending alerts to technicians after a dispatcher assigns them an event – regardless of connection status.
    * Managing a list of technicians with real-time connection status (connected/disconnected).
    * Managing the list of events assigned to each technician.
    * Supporting event transfer from technician to technician (both receive an update).
    * Saving all data and history in the database.

#### C. User Interface (Frontend – Angular)
* Separate interface for each user type, with real-time updates.

### 3. System Users
* **Dispatcher/Manager:** Views all active events , receives immediate alerts (even with browser closed) , assigns events , views dashboard (technician status/events) , transfers events between technicians , closes/prioritizes events , and receives alerts on technician updates.
* **Technician/Field Operator:** Views only assigned events , receives immediate alerts (even with browser closed) , updates event status , requests available events , and sends updates/notes to the dispatcher on active events.

### 4. Real-Time Communication Requirements
* The system requires two separate alert modes:
    * **Mode A (Connected):** Browser open/active; updates arrive immediately without page refresh.
    * **Mode B (Disconnected):** Browser closed; user receives alerts (clicking navigates to relevant screen) .
* The candidate must plan how the server knows the user's status , how transitions between modes are handled , and how subscriptions are maintained.

### 5. Event Management – State Machine
* Define possible states (Initial, Assigned, At least one intermediate state, Closed/Completed, Canceled).
* Additional requirements: Transitions defined in code , status changes saved with timestamp and user , and history displayed in UI.

### 6. Security and Authentication
* Authentication mechanism , clear permission separation (enforced on the server) , authentication of external sources , and encrypted communication on all channels.

### 7. Submission Requirements
* **Architecture Document:** Architectural diagram , component descriptions , Agent details , State Machine , Data model , Security mechanism , behavior during component failure , and trade-offs.
* **E2E Implementation:** Full implementation of the flow: External source → Agent → Server → Database → Dispatcher receives alert .
* **Skeleton:** Clear structure for the rest of the system, defined layers, interfaces, and stubs.
* **Code Quality:** Git repository , README , compile-ready , and Unit Tests for the State Machine.

### Bonus Question – Offline Mode
* Describe at a high level how to support technician work without network access:
    * What data to store locally and how.
    * How to synchronize actions upon reconnection.
    * How to handle conflicts.