CXX = g++
CXXFLAGS = -Wall -Wextra -Wmissing-declarations
LDFLAGS =
OBJFILES = CaffWebApp.Parser.o
TARGET = CaffParser

all: $(TARGET)

$(TARGET): $(OBJFILES)
	$(CXX) $(CXXFLAGS) -o $(TARGET) $(OBJFILES) $(LDFLAGS)
clean:
	rm -f $(OBJFILES) $(TARGET) *~
