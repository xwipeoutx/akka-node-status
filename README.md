# akka-node-status
Example of keeping track of machine connectivity with heartbeats and pings

Description
-----------

This is just a little demo project to show how to keep track of machine connectivity using Akka.NET

The scenario is MSMQ-based service bus connectivity - each node knows what machines are connected to the database, and may or may not receive heartbeats from them (depending on connectivity status).

If a node is known, and has not received a heartbeat in a while, it displays as bad, and the node is pinged to see if there is a network problem.

Of course, it's all fake.
