<body>
    <h1>.NET Minimal API for Distributed System</h1>
    <h2>Setup and Run</h2>
    <ol>
        <li>Clone the repository:</li>
        <pre><code>git clone https://github.com/GeorgeD615/DistributedSystemAPI.git</code></pre>
        <li>Navigate to the project folder:</li>
        <pre><code>cd &lt;project_folder_with_dockerfile&gt;</code></pre>
        <li>Build and start the containers:</li>
        <pre><code>docker build -t api .</code></pre>
        <pre><code>docker run -d -p 127.0.0.1:3000:8080 api</code></pre>
        <li>API will be available at:</li>
        <pre><code>http://localhost:3000</code></pre>
    </ol>
</body>
</html>
