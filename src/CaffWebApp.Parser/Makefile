CXX = g++
CXXFLAGS = -Wall -Wextra -Wmissing-declarations -Werror -pedantic -std=c++11
LDFLAGS =
OBJFILES = CaffWebApp.Parser.o gifanim.o
TARGET = CaffParser

all: $(TARGET)

$(TARGET): $(OBJFILES)
	$(CXX) $(CXXFLAGS) -o $(TARGET) $(OBJFILES) $(LDFLAGS)
clean:
	rm -f $(OBJFILES) $(TARGET) *~
