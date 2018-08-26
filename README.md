# Project Description

The nuXleus Project is comprised of two primary node-types: A client and a server.

# Client

The ciient-side of the Nuxleus Project is a user-centric web application and messaging appliance in which runs within a sandboxed environment on a user-specific host machine (like your laptop or desktop machine.) It acts as a secured proxy,  blip messaging router and subscription engine, and a personal publishing platform that can be used by locally running applications. In other words, instead of renting processor power from someone else, it uses all those Gigahertz of multi-core processing power sitting right there on your computer to do things like render interesting data into web pages that look pretty, or stream media content to TV's, computers, stereo systems, and other web-enabled media devices that are connected to your local and personal network.

Our primary goal for the client-side of Nuxleus is to reduce an applications direct, unfiltered exposure to the Internet to ZERO, using Nuxleus as the primary and only point of contact to the outside web, while acting as a personal, online presence hosting provider. Using this synopsis, Nuxleus can then provide a safe and secured environment to provide extended web-based services built around the overall vision of the Semantic Web*, ensuring that our personal host OS never has the need to risk that which could lead to any type of malware, virus, worm, or other form of threat from the outside world by limiting ALL communications to first pass through the Nuxleus interface, to then be examined for potential threats, checked for proper security credentials, passing the result to the host system if, and only if, ALL security processes in place have given each specific message the green light to be passed through a single secured port on the host machine.

# Server

Just like the client except it implements a many-to-many relationship with client nodes, keeping the entire system flowing regardless of whether any of the client machines are connected at any given time. You know how you can send someone an email and they'll get it regardless of whether or not they are connected to the Internet at that exact moment in time? The server-side of Nuxleus works just like that.

# SPECIAL NOTE

Please keep in mind this project is not intended nor designed to ever act as a stand alone client solution. In other words, the client-side portion of the Nuxleus project has no intention of ever becoming the primary operating system on any given host. Instead, it acts as an integrated solution that works with existing operating systems such as Windows, GNU/Linux, *BSD (e.g. FreeBSD, Mac OS X, NetBSD, etc.), Solaris, AmigaOS, and so-forth, acting as a virtual and secure gateway to the web in which applications running on the host OS can use to securely interact with online applications, but in such a way that if you're ever not connected to the Internet, your locally running applications that utilize Nuxleus won't ever know the difference. Why would they? They never connect to anything other than the locally running Nuxleus instance, so as long as your machine can turn itself on and process bits and bytes, you can be 2000 miles from the nearest WiFi? connection and never know the difference. Your data might be a little out of date until the next time you connect and synchronize with the server, but beyond that, you're connected to the web regardless of whether you're connected to the Internet.

# Technology Overview

## Summary

The Nuxleus Project platform is built on top of a system of simple XML messages that are passed from one process to the next until the original intent of any particular request has either completed successfully or failed completely. The core of this system relies upon the use of a logging mechanism for each sub-process (micro-process) within any given transaction process (macro-process) that insures that once the delivery of any particular message to any particular micro-process has completed successfully, it can forget about everything and move on to handling the next request that comes its direction. In other words, as a process is taking place, the start time, the end time, the parameter names and related values, and the result of the micro-process are sent to a queue server who's only task is to handle the logging of each completed request in such a way that if something goes wrong, the logging system can be queried, the entire process up until the point of failure reconstructed and returned, and the result of that query processed, analyzed, if at all possible fixed by the system itself, and if not, a blip notification sent to a particular URI for further review by a human.

## System Architecture

There are five primary pieces of the Nuxleus Project System Architecture,

 * Nuxleus Core: The System Message Routing & Distribution Engine
 * Xameleon: The Message Processing & Transformation Engine
 * AtomicXML: The Server-side Service-Oriented Atomic Operation Workflow Modeling Language
 * Atomictalk: The Client-side MVC-influenced Communication & Content Rendering Engine
 * Cherokee: A High Performance Highly Secure Multiplexing HTTP/S Server used inside every WiFi-enabled GoPro and used by NASA, the NSA and a myriad of other companies and government agencies.

## Latest Project News

The following is outdated and in desperate need of an update.

Monday, January 1st, 2007

An archive of the first official preview release is available. https://web.archive.org/web/20130914074619/http://www.oreillynet.com/xml/blog/2007/01/annnuxleus_the_nuxleus_project.html

