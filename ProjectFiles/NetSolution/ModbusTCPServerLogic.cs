#region Using directives
using System;
using FTOptix.Core;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.OPCUAServer;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using EasyModbus;
using FTOptix.CommunicationDriver;
#endregion

public class ModbusTCPServerLogic : BaseNetLogic
{
    PeriodicTask myTask;
    EasyModbus.ModbusServer modbusServer;

    public override void Start()
    {
        modbusServer = new EasyModbus.ModbusServer();
        modbusServer.Listen();
        modbusServer.HoldingRegistersChanged += new EasyModbus.ModbusServer.HoldingRegistersChangedHandler(holdingRegistersChanged);
        myTask = new PeriodicTask(MB_Server_Task, 100, LogicObject);
        myTask.Start();
    }

    public void holdingRegistersChanged(int startingAddress, int quantity)
    {
        switch (startingAddress)
        {
            case 1:
                Project.Current.GetVariable("Model/Variable1").Value = modbusServer.holdingRegisters[1];
                break;
            case 2:
                Project.Current.GetVariable("Model/Variable2").Value = modbusServer.holdingRegisters[2];
                break;
            case 3:
                Project.Current.GetVariable("Model/Variable3").Value = modbusServer.holdingRegisters[3];
                break;
            default:
                break;
        }
    }

    public override void Stop()
    {
        myTask.Dispose();
    }

    private void MB_Server_Task()
    {
        modbusServer.holdingRegisters[1] = Project.Current.GetVariable("Model/Variable1").Value;
        modbusServer.holdingRegisters[2] = Project.Current.GetVariable("Model/Variable2").Value;
        modbusServer.holdingRegisters[3] = Project.Current.GetVariable("Model/Variable3").Value;
    }
}
