This is the sample code for the demonstration of sensor recording data. (Initial draft)
We are running multiple sensors here, and each senosr generates some data values, based on some pre-defined condition we have to record some data points.
Data points to be recorded when: 
Any sensor reaches its maximum value, we have to also record the event data which shows the current Maximum sensor reading and previous sensor reading, and their difference
We have to record the maximum value for each sensor, and also maintain the order of occurance.

Added some color coding to highlight conditional event capturing. This can be removed; only for the demo purpose.

This is the initial draft only and can change. I have assumed the following cases:

There is some upper limit for sensor represented by THRESHOLD_VALUE
When any sensor reaches its THRESHOLD_VALUE value we stop reading data from the sensor
When all sensors reach their THRESHOLD_VALUE value then we stop collecting the data points. Then we display the data points for each sensors whenever they reached the their intermidiate max value.
Included Unit Test Cases. 

Functionality implemented is based on the above assumptions.
