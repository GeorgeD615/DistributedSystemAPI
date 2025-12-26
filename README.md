<body>
    <h1>.NET Minimal API — Distributed System (Master / Followers)</h1>
    <p>
        This project demonstrates a simple distributed system built with
        <strong>.NET 8 Minimal API</strong>.
        It uses a <strong>master–followers</strong> architecture, where:
    </p>
    <ul>
        <li>The <strong>master</strong> node accepts write requests</li>
        <li>Updates are represented as <strong>JSON Patch operations</strong></li>
        <li>The master applies the patch locally and propagates it to all followers</li>
        <li>Followers apply the same patch to keep state consistent</li>
        <li>A simple <strong>vector clock</strong> is used to prevent stale updates</li>
    </ul>
    <h2>Architecture Overview</h2>
    <ul>
        <li><strong>Master node</strong> — receives PATCH requests and broadcasts them</li>
        <li><strong>Follower nodes</strong> — receive updates only from the master</li>
        <li>All nodes run the same application image</li>
        <li>Roles and topology are configured via <code>appsettings.json</code> and environment variables</li>
    </ul>
    <h2>Available Endpoints</h2>
    <ul>
        <li>
            <strong>/</strong>  
            <br />
            Returns a simple status message: <code>"Server started!"</code>
        </li>
        <li>
            <strong>/get</strong>  
            <br />
            Returns the current JSON state stored on the node.
        </li>
        <li>
            <strong>/replace</strong>  
            <br />
            Applies a single <strong>JSON Patch operation</strong> to the current state.
            <br />
            On the master node, the patch is also forwarded to all followers.
        </li>
        <li>
            <strong>/vclock</strong>  
            <br />
            Returns the current vector clock (logical timestamps per source).
        </li>
        <li>
            <strong>/test</strong>  
            <br />
            Simple HTML UI for manual testing of <code>/replace</code> and <code>/get</code>.
        </li>
    </ul>
    <h3>Example <code>/replace</code> request body</h3>
    <p>
        The request contains:
    </p>
    <ul>
        <li><strong>Source</strong> — identifier of the request sender</li>
        <li><strong>ID</strong> — monotonically increasing logical timestamp</li>
        <li><strong>Payload</strong> — a single JSON Patch operation (RFC 6902)</li>
    </ul>
    <pre><code>{
    "Source": "Dyagilev",
    "ID": 1,
    "Payload": "{'op': 'add', 'path': '/hello', 'value': { 'name': 'Ginger' }}"
}</code></pre>
    <h2>JSON Patch Examples</h2>
    <pre><code>{
    "op": "add",
    "path": "/user",
    "value": { "name": "Alice" }
}</code></pre>
    <pre><code>{
    "op": "replace",
    "path": "/user/name",
    "value": "Bob"
}</code></pre>
    <pre><code>{
    "op": "remove",
    "path": "/user"
}</code></pre>
    <h2>Setup and Run (Docker Compose)</h2>
    <p>
        The system is designed to run using <strong>Docker Compose</strong>.
        One container acts as the master, and multiple containers act as followers.
    </p>
    <ol>
        <li>
            Clone the repository:
            <pre><code>git clone https://github.com/GeorgeD615/DistributedSystemAPI.git</code></pre>
        </li>
        <li>
            Navigate to the project directory:
            <pre><code>cd DistributedSystemAPI</code></pre>
        </li>
        <li>
            Build and start all services:
            <pre><code>docker compose up --build</code></pre>
        </li>
        <li>
            Open the master node in a browser:
            <pre><code>http://localhost:3000</code></pre>
        </li>
    </ol>
    <h2>Ports Mapping</h2>
    <ul>
        <li><strong>Master:</strong> <code>http://localhost:3000</code></li>
        <li><strong>Follower 1:</strong> <code>http://localhost:3001</code></li>
        <li><strong>Follower 2:</strong> <code>http://localhost:3002</code></li>
        <li><strong>Follower 3:</strong> <code>http://localhost:3003</code></li>
    </ul>
    <h2>Notes</h2>
    <ul>
        <li>The system demonstrates <strong>eventual consistency</strong></li>
        <li>JSON Patch operations are applied in order using logical clocks</li>
        <li>This project is intended for educational purposes</li>
    </ul>
</body>
