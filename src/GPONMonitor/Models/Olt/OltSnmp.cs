﻿using GPONMonitor.Exceptions;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace GPONMonitor.Models.Olt
{
    public partial class Olt
    {
        private async Task<IList<Variable>> SnmpGetAsyncWithTimeout(VersionCode snmpVersion, string oid, int snmpRequestTimeout)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Task<IList<Variable>> task;

            if (await Task.WhenAny(task = SnmpGetAsync(snmpVersion, oid), Task.Delay(snmpRequestTimeout)) == Task.Delay(snmpRequestTimeout))
            {
                cancellationTokenSource.Cancel();
                throw new SnmpTimeoutException("SNMP request timeout");
            }

            return await task;
        }

        private async Task<IList<Variable>> SnmpGetAsync(VersionCode snmpVersion, string oid)
        {
            try
            {
                Task<IList<Variable>> task = Messenger.GetAsync(snmpVersion,
                                    new IPEndPoint(SnmpIPAddress, SnmpPort),
                                    new OctetString(SnmpCommunity),
                                    new List<Variable> { new Variable(new ObjectIdentifier(oid)) });

                return await task;
            }
            catch (Exception exception)
            {
                throw new SnmpConnectionException("SNMP request error: " + exception.Message);
            }
        }

        private async Task<IList<Variable>> SnmpSetAsyncWithTimeout(VersionCode snmpVersion, string oid, string data, int snmpRequestTimeout)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Task<IList<Variable>> task;

            if (await Task.WhenAny(task = SnmpSetAsync(snmpVersion, oid, data), Task.Delay(snmpRequestTimeout)) == Task.Delay(snmpRequestTimeout))
            {
                cancellationTokenSource.Cancel();
                throw new SnmpTimeoutException("SNMP request timeout");
            }

            return await task;
        }

        private async Task<IList<Variable>> SnmpSetAsync(VersionCode snmpVersion, string oid, string data)
        {
            try
            {
                Task<IList<Variable>> task = Messenger.Set(snmpVersion,
                                    new IPEndPoint(SnmpIPAddress, SnmpPort),
                                    new OctetString(SnmpCommunity),
                                    new List<Variable> { new Variable(new ObjectIdentifier(oid), new OctetString(data)) });

                return await task;
            }
            catch (Exception exception)
            {
                throw new SnmpConnectionException("SNMP request error: " + exception.Message);
            }
        }

        private async Task<List<Variable>> SnmpWalkAsyncWithTimeout(VersionCode snmpVersion, string oid, int timeout, WalkMode walkMode, int snmpRequestTimeout)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Task<List<Variable>> task;

            if (await Task.WhenAny(task = SnmpWalkAsync(snmpVersion, oid, timeout, walkMode), Task.Delay(snmpRequestTimeout)) == Task.Delay(snmpRequestTimeout))
            {
                cancellationTokenSource.Cancel();
                throw new SnmpTimeoutException("SNMP request timeout");
            }

            return await task;
        }

        private async Task<List<Variable>> SnmpWalkAsync(VersionCode snmpVersion, string oid, int timeout, WalkMode walkMode)
        {
            List<Variable> snmpWalkResult = new List<Variable>();

            try
            {
                Task<int> taskWalk = Messenger.WalkAsync(snmpVersion,
                                    new IPEndPoint(SnmpIPAddress, SnmpPort),
                                    new OctetString(SnmpCommunity),
                                    new ObjectIdentifier(oid),
                                    snmpWalkResult,
                                    timeout,
                                    walkMode);

                await taskWalk;
                return snmpWalkResult;
            }
            catch (Exception exception)
            {
                throw new SnmpConnectionException("SNMP request error: " + exception.Message);
            }
        }
    }
}
