require(signal)

a = c(1.000000000000000, -3.316807910624417, 4.174245550076570, -2.357402780562255, 0.503375360741703)
b = c(0.000213138726975, 0.000852554907900, 0.001278832361851, 0.000852554907900, 0.000213138726975)

#x = c(1, rep(0, 99)) # This generates the intial input for initial input response

#xt = read.table("ButterworthIn2.txt")

#x = xt[["V1"]]

x = round(runif(300, 0, 1) * 8) / 8

write(x, "ButterworthIn3.txt", ncolumns = 1)

waveform = filter(b, a, x)

write(waveform, "ButterworthOut3.txt", ncolumns = 1)

